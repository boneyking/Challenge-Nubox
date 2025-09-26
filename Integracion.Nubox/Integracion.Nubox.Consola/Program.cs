using ClosedXML.Excel;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Generando archivo Excel de Asistencia con 100,000 registros...");

        var fileName = "Asistencia_100K.xlsx";

        if (File.Exists(fileName))
            File.Delete(fileName);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Asistencia");

            worksheet.Cell(1, 1).Value = "DNI";
            worksheet.Cell(1, 2).Value = "Nombres";
            worksheet.Cell(1, 3).Value = "Apellidos";
            worksheet.Cell(1, 4).Value = "Fecha";
            worksheet.Cell(1, 5).Value = "Hora Entrada";
            worksheet.Cell(1, 6).Value = "Hora Salida";
            worksheet.Cell(1, 7).Value = "Horas Regulares";
            worksheet.Cell(1, 8).Value = "Horas Extras";
            worksheet.Cell(1, 9).Value = "Tipo";
            worksheet.Cell(1, 10).Value = "Observaciones";
            worksheet.Cell(1, 11).Value = "IdExternoPartner";

            var headerRange = worksheet.Range(1, 1, 1, 11);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var nombres = new[] { "Juan", "María", "Pedro", "Ana", "Luis", "Carmen", "José", "Laura", "Carlos", "Sofia" };
            var apellidos = new[] { "García", "Rodríguez", "Martínez", "López", "González", "Pérez", "Sánchez", "Ramírez", "Torres", "Flores" };
            var tipos = new[] { "Presente", "Ausente", "Tardanza", "Licencia", "Vacaciones" };
            var observaciones = new[] { "", "Turno normal", "Horas extra aprobadas", "Licencia médica", "Permiso especial", "Trabajo remoto" };

            var random = new Random();
            var fechaBase = DateTime.Now.Date;

            for (int i = 2; i <= 100001; i++)
            {
                var fechaAsistencia = fechaBase.AddDays(-random.Next(0, 30));
                var horaEntrada = new TimeSpan(8, random.Next(0, 30), 0);
                var horaSalida = horaEntrada.Add(new TimeSpan(8, random.Next(0, 60), 0));

                worksheet.Cell(i, 1).Value = random.Next(10000000, 99999999).ToString();
                worksheet.Cell(i, 2).Value = nombres[random.Next(nombres.Length)];
                worksheet.Cell(i, 3).Value = apellidos[random.Next(apellidos.Length)];

                worksheet.Cell(i, 4).Value = fechaAsistencia;
                worksheet.Cell(i, 4).Style.DateFormat.Format = "yyyy-MM-dd";

                worksheet.Cell(i, 5).Value = horaEntrada;
                worksheet.Cell(i, 5).Style.DateFormat.Format = "hh:mm";

                worksheet.Cell(i, 6).Value = horaSalida;
                worksheet.Cell(i, 6).Style.DateFormat.Format = "hh:mm";

                worksheet.Cell(i, 7).Value = 8m;
                worksheet.Cell(i, 8).Value = random.Next(0, 4);
                worksheet.Cell(i, 9).Value = tipos[random.Next(tipos.Length)];
                worksheet.Cell(i, 10).Value = observaciones[random.Next(observaciones.Length)];
                worksheet.Cell(i, 11).Value = $"EXT-{random.Next(1000, 9999)}";

                if (i % 10000 == 0)
                {
                    Console.WriteLine($"Progreso: {i - 1} registros generados...");
                }
            }

            worksheet.Columns().AdjustToContents();

            Console.WriteLine("\nGuardando archivo...");
            workbook.SaveAs(fileName);

            var fileInfo = new FileInfo(fileName);
            Console.WriteLine($"\n✅ Archivo '{fileName}' generado exitosamente!");
            Console.WriteLine($"📊 Total de registros: 100,000");
            Console.WriteLine($"📁 Tamaño del archivo: {fileInfo.Length / 1024 / 1024} MB");
            Console.WriteLine($"📍 Ubicación: {fileInfo.FullName}");
        }

        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }
}