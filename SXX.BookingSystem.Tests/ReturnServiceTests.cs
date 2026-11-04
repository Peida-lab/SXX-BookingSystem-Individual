
using SXX.BookingSystem.Enums;
using SXX.BookingSystem.Models;
using SXX.BookingSystem.Repositories;
using SXX.BookingSystem.Services;

namespace SXX.BookingSystem.Tests;

public class ReturnServiceTests
{
    // TEST 1 - Lyckad återlämning
    [Fact]
    public void RegisterReturn_WhenBookingIsCheckedOut_ShouldReturnSuccess()
    {
        // Arrange
        var repository = new BookingRepository();
        var service = new ReturnService(repository);

        var booking = new Booking("1001", BookingStatus.CheckedOut);
        repository.AddBooking(booking);

        var returnedAt = DateTimeOffset.Now;

        // Act
        var result = service.RegisterReturn("1001", returnedAt);

        // Assert
        Assert.Equal(ReturnSaveResult.Success, result);
        Assert.Equal(BookingStatus.Returned, booking.Status);
        Assert.Equal(returnedAt, booking.ReturnedAt);
    }

    // TEST 2-4 - Ogiltiga bokningsstatusar
    [Theory]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.Returned)]
    [InlineData(BookingStatus.Cancelled)]
    public void RegisterReturn_WhenStatusIsInvalid_ShouldRejectReturn(
        BookingStatus status)
    {
        // Arrange
        var repository = new BookingRepository();
        var service = new ReturnService(repository);

        var booking = new Booking("1002", status);
        repository.AddBooking(booking);

        // Act
        var result = service.RegisterReturn("1002", DateTimeOffset.Now);

        // Assert
        Assert.Equal(ReturnSaveResult.InvalidStatus, result);
        Assert.Equal(status, booking.Status);
        Assert.Null(booking.ReturnedAt);
    }

    // TEST 5 - Misslyckad lagring
    [Fact]
    public void RegisterReturn_WhenSaveFails_ShouldKeepBookingUnchanged()
    {
        // Arrange
        var repository = new BookingRepository();
        var service = new ReturnService(repository);

        var booking = new Booking("1005", BookingStatus.CheckedOut);
        repository.AddBooking(booking);

        repository.SimulateSaveFailure = true;

        // Act
        var result = service.RegisterReturn("1005", DateTimeOffset.Now);

        // Assert
        Assert.Equal(ReturnSaveResult.SaveFailed, result);
        Assert.Equal(BookingStatus.CheckedOut, booking.Status);
        Assert.Null(booking.ReturnedAt);
    }

    // TEST 6 - Förhindra dubbel återlämning
    [Fact]
    public void RegisterReturn_WhenBookingAlreadyReturned_ShouldRejectSecondReturn()
    {
        // Arrange
        var repository = new BookingRepository();
        var service = new ReturnService(repository);

        var booking = new Booking("1006", BookingStatus.CheckedOut);
        repository.AddBooking(booking);

        var firstReturnTime = DateTimeOffset.Now;

        // Act
        var firstResult = service.RegisterReturn("1006", firstReturnTime);
        var secondResult = service.RegisterReturn(
            "1006",
            firstReturnTime.AddMinutes(10));

        // Assert
        Assert.Equal(ReturnSaveResult.Success, firstResult);
        Assert.Equal(ReturnSaveResult.InvalidStatus, secondResult);
        Assert.Equal(BookingStatus.Returned, booking.Status);
        Assert.Equal(firstReturnTime, booking.ReturnedAt);
    }

    // TEST 7 - Bokningen finns inte
    [Fact]
    public void RegisterReturn_WhenBookingDoesNotExist_ShouldRejectReturn()
    {
        // Arrange
        var repository = new BookingRepository();
        var service = new ReturnService(repository);

        // Act
        var result = service.RegisterReturn("9999", DateTimeOffset.Now);

        // Assert
        Assert.Equal(ReturnSaveResult.InvalidStatus, result);
        Assert.Null(service.GetBooking("9999"));
    }
}
