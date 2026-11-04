
using SXX.BookingSystem.Models;

namespace SXX.BookingSystem.Repositories;

public class BookingRepository
{
    private readonly List<Booking> _bookings = new();

    public void AddBooking(Booking booking)
    {
        _bookings.Add(booking);
    }

    public Booking? GetBooking(string bookingNumber)
    {
        return _bookings.FirstOrDefault(
            booking => booking.BookingNumber == bookingNumber);
    }
}
