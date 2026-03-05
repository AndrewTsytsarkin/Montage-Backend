 
public class UserDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WorkReportDto
{
    public int Id { get; set; }

    public string? UserFullName { get; set; }  // ✅ НОВОЕ
    public int UserId { get; set; }
    public string UserLogin { get; set; } = string.Empty;
    public int ObjectId { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public int WorkTypeId { get; set; }
    public string WorkTypeName { get; set; } = string.Empty;
    public string WorkTypeType { get; set; } = string.Empty;
    public string WorkTypeSubtype { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWorkReportDto
{
    public int ObjectId { get; set; }
    public int WorkTypeId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal Quantity { get; set; }
    public string? Comment { get; set; }
}

public class UpdateWorkReportDto
{
    public decimal? Quantity { get; set; }
    public string? Comment { get; set; }
}

public class WorkTypeDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Subtype { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }  // ✅ Должно быть!

}

public class LoginDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}





public class CreateUserDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = "Worker";
}

public class UpdateUserDto
{
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? Password { get; set; }
}


// Для отображения объекта
public class ProjectObjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Список назначенных пользователей
    public List<AssignedUserDto> AssignedUsers { get; set; } = new();

    // Список всех доступных пользователей (для назначения)
    public List<AvailableUserDto> AvailableUsers { get; set; } = new();
}

// Пользователь, назначенный на объект
public class AssignedUserDto
{
    public int UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string? FullName { get; set; }
}

// Пользователь, доступный для назначения
public class AvailableUserDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public bool IsAssigned { get; set; }  // Уже назначен на этот объект?
}

// Для создания/обновления объекта
public class CreateUpdateObjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = "В работе";
    public string? Description { get; set; }

    // IDs пользователей для назначения
    public List<int>? AssignedUserIds { get; set; }
}