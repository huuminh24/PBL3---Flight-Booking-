$connStr = "Server=LAPTOP-IE8TBPPB\SQLEXPRESS;Database=AirlineBookingDb;Trusted_Connection=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
SELECT f.FlightNumber,
    SUM(CASE WHEN s.SeatClass='Economy'  THEN 1 ELSE 0 END) AS Econ,
    SUM(CASE WHEN s.SeatClass='Business' THEN 1 ELSE 0 END) AS Biz
FROM Flights f
LEFT JOIN Seats s ON s.FlightId = f.Id
GROUP BY f.FlightNumber
ORDER BY f.FlightNumber
"@
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    $econ = $reader.GetInt32(1)
    $biz = $reader.GetInt32(2)
    $flag = ""
    if ($econ -ge 30 -and $biz -ge 12) { $flag = " [OK]" }
    else { $flag = " [FAIL - need more seats]" }
    Write-Host "$($reader.GetString(0)): Econ=$econ Biz=$biz$flag"
}
$conn.Close()