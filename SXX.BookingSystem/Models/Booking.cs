
using SXX.BookingSystem.Enums;

namespace SXX.BookingSystem.Models;

public class Booking
{
    public Guid Id { get; }
    public string BookingNumber { get; }
    public BookingStatus Status { get; private set; }
    public DateTimeOffset? ReturnedAt { get; private set; }

    public Booking(string bookingNumber, BookingStatus status)
    {
        Id = Guid.NewGuid();
        BookingNumber = bookingNumber;
        Status = status;
    }

    public void MarkAsReturned(DateTimeOffset returnedAt)
    {
        if (Status != BookingStatus.CheckedOut)
        {
            throw new InvalidOperationException(
                "Endast utlämnade bokningar kan återlämnas.");
        }

        Status = BookingStatus.Returned;
        ReturnedAt = returnedAt;
    }
}
