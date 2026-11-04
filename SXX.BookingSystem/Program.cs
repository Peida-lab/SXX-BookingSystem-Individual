
using SXX.BookingSystem.Enums;
using SXX.BookingSystem.Models;
using SXX.BookingSystem.Repositories;
using SXX.BookingSystem.Services;

// Skapa repository och service
var repository = new BookingRepository();
var returnService = new ReturnService(repository);

// Testbokningar med olika statusar
repository.AddBooking(new Booking("1001", BookingStatus.CheckedOut));
repository.AddBooking(new Booking("1002", BookingStatus.Confirmed));
repository.AddBooking(new Booking("1003", BookingStatus.Returned));
repository.AddBooking(new Booking("1004", BookingStatus.Cancelled));

Console.WriteLine("=== SXX Bokningssystem ===");
Console.WriteLine("Registrera återlämning");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Testbokningar ===");
    Console.WriteLine("1001 - Utlämnad från början");
    Console.WriteLine("1002 - Bekräftad");
    Console.WriteLine("1003 - Redan återlämnad");
    Console.WriteLine("1004 - Avbokad");
    Console.WriteLine("0 - Avsluta programmet");
    Console.WriteLine();

    Console.Write("Ange bokningsnummer: ");
    string bookingNumber = (Console.ReadLine() ?? "").Trim();

    if (bookingNumber == "0")
    {
        Console.WriteLine("Programmet avslutas.");
        break;
    }

    var foundBooking = returnService.GetBooking(bookingNumber);

    if (foundBooking is null)
    {
        Console.WriteLine("Bokningen hittades inte.");
        continue;
    }

    Console.WriteLine($"Bokningsnummer: {foundBooking.BookingNumber}");
    Console.WriteLine($"Status: {foundBooking.Status}");

    if (foundBooking.Status != BookingStatus.CheckedOut)
    {
        Console.WriteLine("Uthyrningen är inte påbörjad eller är redan avslutad.");
        continue;
    }

    Console.Write("Vill du registrera återlämningen? (j/n): ");
    string answer = (Console.ReadLine() ?? "").Trim();

    if (!answer.Equals("j", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Återlämningen avbröts.");
        continue;
    }

    var result = returnService.RegisterReturn(
        bookingNumber,
        DateTimeOffset.Now);

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
}
