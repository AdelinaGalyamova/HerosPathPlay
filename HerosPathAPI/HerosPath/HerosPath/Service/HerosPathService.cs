using HerosPath.DatabaseContext;
using HerosPath.Interfaces;
using HerosPath.Models;
using HerosPath.Requests;
using Microsoft.EntityFrameworkCore;

namespace HerosPath.Service
{
    public class HerosPathService : IHerosPathService
    {
        private readonly ContextDb _context;

        public HerosPathService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<object> Register(RegisterRequest request)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null)
            {
                return new { status = false, message = "Пользователь с такой почтой уже существует" };
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string token = Guid.NewGuid().ToString();
            var session = new Session
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            var level1Progress = new LevelProgress
            {
                UserId = user.Id,
                LevelId = 1,
                Stars = 0,
                BestMoves = 0,
                IsCompleted = false,
                IsUnlocked = true // 1 уровень открыт
            };

            _context.LevelsProgress.Add(level1Progress);
            await _context.SaveChangesAsync();

            return new
            {
                status = true,
                message = "Регистрация успешна",
                token = token,
                userId = user.Id,
                username = user.Username,
                email = user.Email
            };
        }

        public async Task<object> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null)
            {
                return new { status = false, message = "Неверный логин или пароль" };
            }

            var oldSessions = _context.Sessions.Where(s => s.UserId == user.Id);
            foreach (var session in oldSessions)
            {
                session.IsActive = false;
            }

            string token = Guid.NewGuid().ToString();
            var newSession = new Session
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsActive = true
            };

            _context.Sessions.Add(newSession);
            await _context.SaveChangesAsync();

            return new
            {
                status = true,
                message = "Вход выполнен",
                token = token,
                userId = user.Id,
                username = user.Username
            };
        }

        public async Task<object> Logout(string token)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session != null)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
            }
            return new { status = true, message = "Выход выполнен" };
        }

        public async Task<object> GetUser(string token)
        {
            var session = await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Token == token && s.IsActive);

            if (session == null)
            {
                return new { status = false, message = "Неверный токен" };
            }

            if (session.ExpiresAt < DateTime.UtcNow)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
                return new { status = false, message = "Токен истёк" };
            }

            return new
            {
                status = true,
                userId = session.User.Id,
                username = session.User.Username,
                email = session.User.Email
            };
        }

        public async Task<object> GetLevels(string token)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token && s.IsActive);
            if (session == null)
                return new { status = false, message = "Неверный токен" };

            var levels = await _context.Levels.OrderBy(l => l.OrderIndex).ToListAsync();
            var progress = await _context.LevelsProgress
                .Where(p => p.UserId == session.UserId)
                .ToDictionaryAsync(p => p.LevelId);

            var result = levels.Select(level => new
            {
                level.Id,
                level.Name,
                level.LevelNumber,
                IsLocked = level.Id == 1 ? false : !(progress.ContainsKey(level.Id) && progress[level.Id].IsUnlocked),
                Progress = progress.ContainsKey(level.Id) ? new
                {
                    stars = progress[level.Id].Stars,
                    bestMoves = progress[level.Id].BestMoves,
                    isCompleted = progress[level.Id].IsCompleted
                } : null
            });

            return new { status = true, levels = result };
        }

        public async Task<object> GetProgress(string token)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token && s.IsActive);
            if (session == null)
            {
                return new { status = false, message = "Неверный токен" };
            }

            var progress = await _context.LevelsProgress.Include(p => p.Level).Where(p => p.UserId == session.UserId).ToListAsync();

            int totalStars = progress.Sum(p => p.Stars);
            int completedLevels = progress.Count(p => p.IsCompleted);

            return new
            {
                status = true,
                totalStars = totalStars,
                completedLevels = completedLevels,
                totalLevels = await _context.Levels.CountAsync(),
                progress = progress.Select(p => new
                {
                    levelId = p.LevelId,
                    levelName = p.Level.Name,
                    stars = p.Stars,
                    bestMoves = p.BestMoves,
                    isCompleted = p.IsCompleted,
                    completedAt = p.CompletedAt
                })
            };
        }

        public async Task<object> SaveProgress(SaveProgressRequest request)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == request.Token && s.IsActive);
            if (session == null)
                return new { status = false, message = "Неверный токен" };

            var levelExists = await _context.Levels.AnyAsync(l => l.Id == request.LevelId);
            if (!levelExists)
                return new { status = false, message = $"Уровень {request.LevelId} не найден" };

            var existing = await _context.LevelsProgress.FirstOrDefaultAsync(p => p.UserId == session.UserId && p.LevelId == request.LevelId);

            if (existing == null)
            {
                var newProgress = new LevelProgress
                {
                    UserId = session.UserId,
                    LevelId = request.LevelId,
                    Stars = request.Stars,
                    BestMoves = request.Moves,
                    IsCompleted = true,
                    IsUnlocked = true,
                    CompletedAt = DateTime.UtcNow
                };
                _context.LevelsProgress.Add(newProgress);
            }
            else
            {
                if (request.Stars > existing.Stars)
                    existing.Stars = request.Stars;
                if (existing.BestMoves == 0 || request.Moves < existing.BestMoves)
                    existing.BestMoves = request.Moves;

                if (!existing.IsCompleted)
                {
                    existing.IsCompleted = true;
                    existing.CompletedAt = DateTime.UtcNow;
                }
                existing.IsUnlocked = true;
                _context.LevelsProgress.Update(existing);
            }

            int nextLevelId = request.LevelId + 1;
            var nextLevelExists = await _context.Levels.AnyAsync(l => l.Id == nextLevelId);

            if (nextLevelExists)
            {
                var nextProgress = await _context.LevelsProgress.FirstOrDefaultAsync(p => p.UserId == session.UserId && p.LevelId == nextLevelId);
                if (nextProgress == null)
                {
                    var newUnlocked = new LevelProgress
                    {
                        UserId = session.UserId,
                        LevelId = nextLevelId,
                        Stars = 0,
                        BestMoves = 0,
                        IsCompleted = false,
                        IsUnlocked = true
                    };
                    _context.LevelsProgress.Add(newUnlocked);
                }
                else
                {
                    nextProgress.IsUnlocked = true;
                    _context.LevelsProgress.Update(nextProgress);
                }
            }

            await _context.SaveChangesAsync();

            int totalStars = await _context.LevelsProgress.Where(p => p.UserId == session.UserId).SumAsync(p => p.Stars);

            return new
            {
                status = true,
                message = $"Уровень {request.LevelId} пройден! Получено {request.Stars} звёзд за {request.Moves} ходов.",
                totalStars = totalStars
            };
        }
    }
}