
using Microsoft.EntityFrameworkCore;

namespace MontageAPI.Data;

public class WorkReport
{
    public int Id { get; set; }

    /// <summary>
    /// Монтажник, выполнивший работу
    /// </summary>
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>
    /// Объект, на котором выполнялась работа
    /// </summary>
    public int ObjectId { get; set; }
    public ProjectObject Object { get; set; } = null!;

    /// <summary>
    /// Вид работы из справочника
    /// </summary>
    public int WorkTypeId { get; set; }
    public WorkType WorkType { get; set; } = null!;

    /// <summary>
    /// Дата выполнения работы
    /// </summary>
    public DateTime WorkDate { get; set; }

    /// <summary>
    /// Количество выполненных единиц работы
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Единица измерения (копируется из справочника для истории)
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Комментарий монтажника
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Дата создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего изменения
    /// </summary>
    public DateTime? UpdatedAt { get; set; }



    /// <summary>
    /// Цена за единицу (копируется из справочника на момент создания отчета)
    /// </summary>
    public decimal PricePerUnit { get; set; }

    /// <summary>
    /// Итоговая стоимость = PricePerUnit × Quantity
    /// </summary>
    public decimal TotalPrice { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Worker"; // Admin или Worker
    /// <summary>
    /// ФИО пользователя (Фамилия Имя Отчество)
    /// </summary>
    public string FullName { get; set; }
    public ICollection<UserObjectAssignment> Assignments { get; set; } = new List<UserObjectAssignment>();
}
public class UserObjectAssignment
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int ObjectId { get; set; }
    public ProjectObject Object { get; set; } = null!;
}
public class ProjectObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public ICollection<UserObjectAssignment> Assignments { get; set; } = new List<UserObjectAssignment>();
}
public class WorkType
{
    public int Id { get; set; }

    /// <summary>
    /// Тип видов работ (например: "Сверление, штробление и подрозетники")
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Подтип видов работ (например: "Отверстия")
    /// </summary>
    public string Subtype { get; set; } = string.Empty;

    /// <summary>
    /// Наименование вида работ (полное описание)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Единица измерения (шт, м, м², компл и т.д.)
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Порядок сортировки внутри подтипа
    /// </summary>
    public int SortOrder { get; set; }



    /// <summary>
    /// Цена за единицу работы (в рублях)
    /// </summary>
    public decimal PricePerUnit { get; set; }
}


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectObject> ProjectObjects => Set<ProjectObject>();
    public DbSet<UserObjectAssignment> UserObjectAssignments => Set<UserObjectAssignment>();
    public DbSet<WorkType> WorkTypes => Set<WorkType>();
    public DbSet<WorkReport> WorkReports => Set<WorkReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === КОНФИГУРАЦИЯ СВЯЗЕЙ ===
        modelBuilder.Entity<UserObjectAssignment>()
            .HasKey(a => new { a.UserId, a.ObjectId });

        modelBuilder.Entity<UserObjectAssignment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserObjectAssignment>()
            .HasOne(a => a.Object)
            .WithMany(o => o.Assignments)
            .HasForeignKey(a => a.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkReport>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkReport>()
            .HasOne(r => r.Object)
            .WithMany()
            .HasForeignKey(r => r.ObjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkReport>()
            .HasOne(r => r.WorkType)
            .WithMany()
            .HasForeignKey(r => r.WorkTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkType>()
            .HasIndex(w => new { w.Type, w.Subtype });

        modelBuilder.Entity<WorkReport>()
            .HasIndex(r => new { r.UserId, r.WorkDate });

        modelBuilder.Entity<WorkReport>()
            .HasIndex(r => new { r.ObjectId, r.WorkDate });

        modelBuilder.Entity<WorkReport>()
            .Property(r => r.Quantity)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WorkReport>()
            .Property(r => r.PricePerUnit)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WorkReport>()
            .Property(r => r.TotalPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WorkType>()
            .Property(w => w.PricePerUnit)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WorkReport>()
            .Property(r => r.Unit)
            .HasMaxLength(10);

        modelBuilder.Entity<WorkReport>()
            .Property(r => r.Comment)
            .HasMaxLength(1000);

        // === ВЫЗОВ МЕТОДОВ SEED ===
        SeedUsers(modelBuilder);
        SeedWorkTypes(modelBuilder);
        SeedProjectObjects(modelBuilder);
        SeedObjectAssignments(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                FullName = "Админ Админыч"

            },
            new User
            {
                Id = 2,
                Login = "worker1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("worker123"),
                Role = "Worker",
                FullName = "Иванов Д.Д."
            },
            new User
            {
                Id = 3,
                Login = "worker2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("worker123"),
                Role = "Worker",
                FullName = "Сидоров И.Д."

            }
        );
    }

    private static void SeedWorkTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkType>().HasData(
            new WorkType
            {
                Id = 1,
                Type = "Сверление, штробление и подрозетники",
                Subtype = "Отверстия",
                Name = "Устройство (сверление) сквозных отверстий в стенах из кирпича, железобетонных перекрытиях, сенгвиче, сенгвич-панелях, металлических конструкциях прочих перегородках и стеновых конструкциях диаметром до 22 мм, глубиной до 1м, на высоте до 4м.",
                Unit = "шт",
                SortOrder = 1,
                PricePerUnit = 150.00m
            },
            new WorkType
            {
                Id = 2,
                Type = "Сверление, штробление и подрозетники",
                Subtype = "Отверстия",
                Name = "Устройство (сверление) сквозных отверстий в стенах из кирпича, железобетонных перекрытиях, сенгвиче, сенгвич-панелях, прочих перегородках и стеновых конструкциях диаметром до 42 мм, глубиной до 1м, на высоте до 4м.",
                Unit = "шт",
                SortOrder = 2,
                PricePerUnit = 250.00m
            },
            new WorkType
            {
                Id = 3,
                Type = "Сверление, штробление и подрозетники",
                Subtype = "Отверстия",
                Name = "Устройство (сверление) сквозных отверстий в стенах из кирпича, железобетонных перекрытиях, сенгвиче, сенгвич-панелях, прочих перегородках и стеновых конструкциях диаметром до 65 мм, глубиной до 380мм, на высоте до 4м.",
                Unit = "шт",
                SortOrder = 3,
                PricePerUnit = 400.00m
            },
            new WorkType
            {
                Id = 4,
                Type = "Сверление, штробление и подрозетники",
                Subtype = "Отверстия",
                Name = "Устройство (сверление) сквозных отверстий в металлических конструкциях (швелерах, метал. листах, лотках, крышках, профлистах, перегородках, существующих электрощитах, коммутационных щкафах) диаметром до 25 мм, глубиной до 6мм, на высоте до 4м.",
                Unit = "шт",
                SortOrder = 4,
                PricePerUnit = 200.00m
            }
        );
    }

    private static void SeedProjectObjects(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectObject>().HasData(
            new ProjectObject
            {
                Id = 1,
                Name = "ТЦ «Галерея» — 3 этаж",
                Address = "г. Москва, Лиговский пр-т, д. 30А",
                Status = "В работе"
            },
            new ProjectObject
            {
                Id = 2,
                Name = "Офисный центр «Сити Плаза»",
                Address = "г. Москва, Пресненская наб., д. 12",
                Status = "В работе"
            },
            new ProjectObject
            {
                Id = 3,
                Name = "Складской комплекс «Логистик»",
                Address = "г. Подольск, ул. Индустриальная, д. 45",
                Status = "Новый"
            },
            new ProjectObject
            {
                Id = 4,
                Name = "ЖК «Солнечный» — Корпус 5",
                Address = "г. Химки, ул. Молодежная, д. 78",
                Status = "Завершен"
            },
            new ProjectObject
            {
                Id = 5,
                Name = "Ресторан «Белая Дача»",
                Address = "г. Котельники, мкр. Белая Дача, д. 35",
                Status = "В работе"
            }
        );
    }

    private static void SeedObjectAssignments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserObjectAssignment>().HasData(
            new UserObjectAssignment { UserId = 2, ObjectId = 1 },
            new UserObjectAssignment { UserId = 2, ObjectId = 2 },
            new UserObjectAssignment { UserId = 2, ObjectId = 3 },
            new UserObjectAssignment { UserId = 3, ObjectId = 3 },
            new UserObjectAssignment { UserId = 3, ObjectId = 4 },
            new UserObjectAssignment { UserId = 3, ObjectId = 5 }
        );
    }
}