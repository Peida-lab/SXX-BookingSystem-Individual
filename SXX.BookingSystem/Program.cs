
using SXX.BookingSystem.Enums;
using SXX.BookingSystem.Models;
using SXX.BookingSystem.Repositories;
using SXX.BookingSystem.Services;

// Skapa repository och service
var repository = new BookingRepository();
var returnService = new ReturnService(repository);

// Skapa en testbokning som redan är utlämnad
var booking = new Booking("1001", BookingStatus.CheckedOut);
repository.AddBooking(booking);

Console.WriteLine("=== SXX Bokningssystem ===");
Console.WriteLine("Registrera återlämning");
Console.WriteLine();

Console.Write("Ange bokningsnummer: ");
string bookingNumber = Console.ReadLine() ?? "";

// Hämta bokningen
var foundBooking = returnService.GetBooking(bookingNumber);

if (foundBooking is null)
{
    Console.WriteLine("Bokningen hittades inte.");
    return;
}

Console.WriteLine($"Bokningsnummer: {foundBooking.BookingNumber}");
Console.WriteLine($"Status: {foundBooking.Status}");

if (foundBooking.Status != BookingStatus.CheckedOut)
{
    Console.WriteLine("Uthyrningen är inte påbörjad.");
    return;
}

// Bekräfta återlämning
Console.Write("Vill du registrera återlämningen? (j/n): ");
string answer = Console.ReadLine() ?? "";

if (answer.ToLower() != "j")
{
    Console.WriteLine("Återlämningen avbröts.");
    return;
}

// Registrera återlämningen
var result = returnService.RegisterReturn(
    bookingNumber,
    DateTimeOffset.Now);

// Hantera resultat
switch (result)
{
    case ReturnSaveResult.Success:
        Console.WriteLine("Återlämningen har registrerats!");
        Console.WriteLine($"Ny status: {foundBooking.Status}");
        Console.WriteLine($"Återlämningstid: {foundBooking.ReturnedAt}");
        break;

    case ReturnSaveResult.InvalidStatus:
        Console.WriteLine("Återlämningen kan inte registreras.");
        break;

    case ReturnSaveResult.SaveFailed:
        Console.WriteLine("Återlämningen kunde inte sparas.");
        break;
}
