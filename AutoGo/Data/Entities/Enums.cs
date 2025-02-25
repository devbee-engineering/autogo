namespace AutoGo.Data.Entities;

public enum UserType
{
    Admin = 1, // Can only create bookings
    Driver = 2, // Can only receive bookings
    SuperDriver = 3 // Can create and receive bookings
}

public enum BookingStatus
{
    Pending = 1,
    Assigned = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}
