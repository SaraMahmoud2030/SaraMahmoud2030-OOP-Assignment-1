using System;
using System.Collections.Generic;

namespace Part2_HotelReservationSystem;

public enum RoomType
{
    Single,
    Double,
    Suite
}

public enum ReservationStatus
{
    Pending,
    Confirmed,
    CheckedIn,
    CheckedOut,
    Cancelled
}

public class Room
{
    public int RoomNumber { get; }
    public RoomType RoomType { get; }
    public decimal NightlyRate { get; private set; }
    public bool IsUnderMaintenance { get; private set; }

    public Room(int roomNumber, RoomType roomType, decimal nightlyRate)
    {
        if (nightlyRate <= 0)
            throw new ArgumentException("Nightly rate must be positive.");

        RoomNumber = roomNumber;
        RoomType = roomType;
        NightlyRate = nightlyRate;
        IsUnderMaintenance = false;
    }

    public void UpdateNightlyRate(decimal newRate)
    {
        if (newRate <= 0)
            throw new ArgumentException("Nightly rate must be positive.");

        NightlyRate = newRate;
    }

    public void StartMaintenance()
    {
        IsUnderMaintenance = true;
    }

    public void EndMaintenance()
    {
        IsUnderMaintenance = false;
    }
}

public class Guest
{
    private readonly List<Reservation> _reservations = new();

    public int GuestId { get; }
    public string FullName { get; }
    public string PhoneNumber { get; }

    public IReadOnlyList<Reservation> Reservations => _reservations;

    public Guest(int guestId, string fullName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty.");

        GuestId = guestId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }

    public void AddReservation(Reservation reservation)
    {
        if (reservation == null)
            throw new ArgumentNullException(nameof(reservation));

        if (reservation.Guest != this)
            throw new InvalidOperationException(
                "This reservation does not belong to this guest.");

        _reservations.Add(reservation);
    }
}

public class Reservation
{
    public int ReservationId { get; }
    public DateTime CheckInDate { get; }
    public DateTime CheckOutDate { get; }
    public Room Room { get; }
    public Guest Guest { get; }
    public ReservationStatus Status { get; private set; }

    public Reservation(
        int reservationId,
        Guest guest,
        Room room,
        DateTime checkInDate,
        DateTime checkOutDate)
    {
        if (guest == null)
            throw new ArgumentNullException(nameof(guest));

        if (room == null)
            throw new ArgumentNullException(nameof(room));

        if (checkOutDate <= checkInDate)
            throw new ArgumentException(
                "Check-out date must be after check-in date.");

        if (room.IsUnderMaintenance)
            throw new InvalidOperationException(
                "Cannot create a reservation for a room under maintenance.");

        ReservationId = reservationId;
        Guest = guest;
        Room = room;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Status = ReservationStatus.Pending;
    }

    public decimal CalculateTotalCost()
    {
        int nights = (CheckOutDate - CheckInDate).Days;
        return nights * Room.NightlyRate;
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidOperationException(
                "Only pending reservations can be confirmed.");

        Status = ReservationStatus.Confirmed;
    }

    public void CheckIn()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException(
                "Only confirmed reservations can be checked in.");

        Status = ReservationStatus.CheckedIn;
    }
    public void CheckOut()
    {
        if (Status != ReservationStatus.CheckedIn)
            throw new InvalidOperationException(
                "Only checked-in reservations can be checked out.");

        Status = ReservationStatus.CheckedOut;
    }

    public void Cancel()
    {
        if (Status != ReservationStatus.Pending &&
            Status != ReservationStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only pending or confirmed reservations can be cancelled.");
        }

        Status = ReservationStatus.Cancelled;
    }
}

public class Hotel
{
    private readonly List<Guest> _guests = new();
    private readonly List<Room> _rooms = new();
    private readonly List<Reservation> _reservations = new();

    public IReadOnlyList<Guest> Guests => _guests;
    public IReadOnlyList<Room> Rooms => _rooms;
    public IReadOnlyList<Reservation> Reservations => _reservations;

    public void AddGuest(Guest guest)
    {
        if (guest == null)
            throw new ArgumentNullException(nameof(guest));

        if (_guests.Exists(g => g.GuestId == guest.GuestId))
            throw new InvalidOperationException(
                "Guest ID must be unique.");

        _guests.Add(guest);
    }

    public void AddRoom(Room room)
    {
        if (room == null)
            throw new ArgumentNullException(nameof(room));

        if (_rooms.Exists(r => r.RoomNumber == room.RoomNumber))
            throw new InvalidOperationException(
                "Room number must be unique.");

        _rooms.Add(room);
    }

    public Reservation CreateReservation(
        int reservationId,
        Guest guest,
        Room room,
        DateTime checkInDate,
        DateTime checkOutDate)
    {
        if (_reservations.Exists(r => r.ReservationId == reservationId))
            throw new InvalidOperationException(
                "Reservation ID must be unique.");

        if (!_guests.Contains(guest))
            throw new InvalidOperationException(
                "Guest must be registered in the hotel.");

        if (!_rooms.Contains(room))
            throw new InvalidOperationException(
                "Room must belong to the hotel.");

        if (HasOverlappingReservation(
            room,
            checkInDate,
            checkOutDate))
        {
            throw new InvalidOperationException(
                "The room is already booked for an overlapping period.");
        }

        Reservation reservation = new Reservation(
            reservationId,
            guest,
            room,
            checkInDate,
            checkOutDate);

        _reservations.Add(reservation);
        guest.AddReservation(reservation);

        return reservation;
    }

    private bool HasOverlappingReservation(
        Room room,
        DateTime checkInDate,
        DateTime checkOutDate)
    {
        foreach (Reservation reservation in _reservations)
        {
            if (reservation.Room != room)
                continue;

            if (reservation.Status == ReservationStatus.Cancelled ||
                reservation.Status == ReservationStatus.CheckedOut)
                continue;

            bool overlaps =
                checkInDate < reservation.CheckOutDate &&
                checkOutDate > reservation.CheckInDate;

            if (overlaps)
                return true;
        }

        return false;
    }
}

public class Program
{
    public static void Main()
    {
        Hotel hotel = new Hotel();

        Room room101 = new Room(
            101,
            RoomType.Single,
            1000m);

        Room room102 = new Room(
            102,
            RoomType.Double,
            1500m);
        Room room201 = new Room(
            201,
            RoomType.Suite,
            2500m);

        hotel.AddRoom(room101);
        hotel.AddRoom(room102);
        hotel.AddRoom(room201);

        Guest guest1 = new Guest(
            1,
            "Sara Mahmoud",
            "01012345678");

        Guest guest2 = new Guest(
            2,
            "Mona Ali",
            "01198765432");

        hotel.AddGuest(guest1);
        hotel.AddGuest(guest2);

        Reservation reservation1 = hotel.CreateReservation(
            1001,
            guest1,
            room101,
            new DateTime(2026, 10, 1),
            new DateTime(2026, 10, 4));

        Console.WriteLine("Reservation created successfully.");
        Console.WriteLine($"Guest: {guest1.FullName}");
        Console.WriteLine($"Room: {reservation1.Room.RoomNumber}");
        Console.WriteLine($"Status: {reservation1.Status}");
        Console.WriteLine($"Total: {reservation1.CalculateTotalCost()}");

        Console.WriteLine();

        reservation1.Confirm();
        Console.WriteLine($"After confirmation: {reservation1.Status}");

        reservation1.CheckIn();
        Console.WriteLine($"After check-in: {reservation1.Status}");

        reservation1.CheckOut();
        Console.WriteLine($"After check-out: {reservation1.Status}");

        Console.WriteLine();
        Console.WriteLine("Guest Reservation History:");

        foreach (Reservation reservation in guest1.Reservations)
        {
            Console.WriteLine(
                $"Reservation {reservation.ReservationId} | " +
                $"Room {reservation.Room.RoomNumber} | " +
                $"{reservation.CheckInDate:yyyy-MM-dd} -> " +
                $"{reservation.CheckOutDate:yyyy-MM-dd} | " +
                $"Status: {reservation.Status} | " +
                $"Total: {reservation.CalculateTotalCost()}");
        }

        Console.WriteLine();
        Console.WriteLine("Testing maintenance:");

        room102.StartMaintenance();

        try
        {
            hotel.CreateReservation(
                1002,
                guest2,
                room102,
                new DateTime(2026, 10, 5),
                new DateTime(2026, 10, 7));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine();
        Console.WriteLine("Testing double booking:");

        try
        {
            hotel.CreateReservation(
                1003,
                guest2,
                room101,
                new DateTime(2026, 10, 2),
                new DateTime(2026, 10, 5));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
 