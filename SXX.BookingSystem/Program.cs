
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

    // Visa bokningarnas aktuella status
    foreach (string number in new[] { "1001", "1002", "1003", "1004" })
    {
        var booking = returnService.GetBooking(number);
        Console.WriteLine($"{number} - {booking?.Status}");
    }

    Console.WriteLine("0 - Avsluta programmet");
    Console.WriteLine();

    Console.Write("Ange bokningsnummer: ");
    string bookingNumber = (Console.ReadLine() ?? "").Trim();

    if (bookingNumber == "0")
    {
        Console.WriteLine("Programmet avslutas.");
        break;
    }

    if (string.IsNullOrWhiteSpace(bookingNumber))
    {
        Console.WriteLine("Du måste ange ett bokningsnummer.");
        continue;
    }

    // Hämta bokningen
    var foundBooking = returnService.GetBooking(bookingNumber);

    if (foundBooking is null)
    {
        Console.WriteLine($"Ingen bokning med nummer {bookingNumber} hittades.");
        continue;
    }

    Console.WriteLine($"Bokningsnummer: {foundBooking.BookingNumber}");
    Console.WriteLine($"Status: {foundBooking.Status}");

    // Kontrollera bokningens status
    switch (foundBooking.Status)
    {
        case BookingStatus.Confirmed:
            Console.WriteLine("Bokningen är bekräftad men utrustningen har inte lämnats ut.");
            continue;

        case BookingStatus.Returned:
            Console.WriteLine("Den här bokningen är redan återlämnad.");
            continue;

        case BookingStatus.Cancelled:
            Console.WriteLine("Bokningen är avbokad och kan inte återlämnas.");
            continue;

        case BookingStatus.CheckedOut:
            break;

        default:
            Console.WriteLine("Bokningen har en ogiltig status.");
            continue;
    }

    // Bekräfta återlämning
    Console.Write("Vill du registrera återlämningen? (ja/nej): ");
    string answer = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (answer == "n" || answer == "nej")
    {
        Console.WriteLine("Återlämningen avbröts.");
        continue;
    }

    if (answer != "j" && answer != "ja")
    {
        Console.WriteLine("Ogiltigt svar. Skriv ja eller nej.");
        continue;
    }

    // Registrera återlämningen
    var result = returnService.RegisterReturn(
        bookingNumber,
        DateTimeOffset.Now);

    // Visa resultat
    switch (result)
    {
        case ReturnSaveResult.Success:
            Console.WriteLine("Återlämningen har registrerats!");
            Console.WriteLine($"Ny status: {foundBooking.Status}");
            Console.WriteLine($"Återlämningstid: {foundBooking.ReturnedAt}");
            break;

        case ReturnSaveResult.InvalidStatus:
            Console.WriteLine("Återlämningen kunde inte registreras eftersom bokningens status har ändrats.");
            break;

        case ReturnSaveResult.SaveFailed:
            Console.WriteLine("Ett fel uppstod vid lagringen. Återlämningen har inte registrerats.");
            break;
    }
}
