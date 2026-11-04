
using SXX.BookingSystem.Enums;
using SXX.BookingSystem.Models;
using SXX.BookingSystem.Repositories;
using SXX.BookingSystem.Services;

// Skapa repository och service
var repository = new BookingRepository();
var returnService = new ReturnService(repository);

// Skapa testbokningar med olika statusar
repository.AddBooking(new Booking("1001", BookingStatus.CheckedOut));
repository.AddBooking(new Booking("1002", BookingStatus.Confirmed));
repository.AddBooking(new Booking("1003", BookingStatus.Returned));
repository.AddBooking(new Booking("1004", BookingStatus.Cancelled));

// Starta programmet
Console.WriteLine("=== SXX Bokningssystem ===");
Console.WriteLine("Registrera återlämning");
Console.WriteLine();

Console.WriteLine("=== Testbokningar ===");
Console.WriteLine("1001 - Utlämnad (CheckedOut)");
Console.WriteLine("1002 - Bekräftad (Confirmed)");
Console.WriteLine("1003 - Återlämnad (Returned)");
Console.WriteLine("1004 - Avbokad (Cancelled)");
Console.WriteLine();

// Ange bokningsnummer
Console.Write("Ange bokningsnummer: ");
string bookingNumber = (Console.ReadLine() ?? "").Trim();

// Hämta bokningen
var foundBooking = returnService.GetBooking(bookingNumber);

if (foundBooking is null)
{
    Console.WriteLine("Bokningen hittades inte.");
    return;
}

// Visa bokningsinformation
Console.WriteLine($"Bokningsnummer: {foundBooking.BookingNumber}");
Console.WriteLine($"Status: {foundBooking.Status}");

// Kontrollera bokningsstatus
if (foundBooking.Status != BookingStatus.CheckedOut)
{
    Console.WriteLine("Uthyrningen är inte påbörjad eller är redan avslutad.");
    return;
}

// Bekräfta återlämning
Console.Write("Vill du registrera återlämningen? (j/n): ");
string answer = (Console.ReadLine() ?? "").Trim();

if (!answer.Equals("j", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Återlämningen avbröts.");
    return;
}

// Registrera återlämningen
var result = returnService.RegisterReturn(
    bookingNumber,
    DateTimeOffset.Now);

// Visa resultatet
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
