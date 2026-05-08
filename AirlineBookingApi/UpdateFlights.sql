-- Delete old seeded flights with 3xxx, 4xxx, 5xxx flight numbers
DELETE FROM Seats WHERE FlightId IN (
    SELECT Id FROM Flights
    WHERE CONVERT(date, DepartureTime) IN ('2026-05-07','2026-05-08','2026-05-09')
    AND (FlightNumber LIKE '3[1-4]%' OR FlightNumber LIKE '4[1-4]%' OR FlightNumber LIKE '5[1-4]%')
);

DELETE FROM FlightPrices WHERE FlightId IN (
    SELECT Id FROM Flights
    WHERE CONVERT(date, DepartureTime) IN ('2026-05-07','2026-05-08','2026-05-09')
    AND (FlightNumber LIKE '3[1-4]%' OR FlightNumber LIKE '4[1-4]%' OR FlightNumber LIKE '5[1-4]%')
);

DELETE FROM Flights
WHERE CONVERT(date, DepartureTime) IN ('2026-05-07','2026-05-08','2026-05-09')
AND (FlightNumber LIKE '3[1-4]%' OR FlightNumber LIKE '4[1-4]%' OR FlightNumber LIKE '5[1-4]%');

PRINT 'Old flights deleted successfully.';
