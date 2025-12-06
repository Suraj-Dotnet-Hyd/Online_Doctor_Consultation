using PrescriptionService.DTOs;
using PrescriptionService.Models;
using PrescriptionService.Repository;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Text.Json;

namespace PrescriptionService.Service
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescription _repo;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public PrescriptionService(IPrescription repo, IWebHostEnvironment env, IConfiguration config)
        {
            _repo = repo;
            _env = env;
            _config = config;
        }

        //public async Task<PrescriptionResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto dto, HttpRequest request)
        //{
        //    // prepare entity
        //    var entity = new Prescription
        //    {
        //        AppointmentId = dto.AppointmentId,
        //        PatientId = dto.PatientId,
        //        DoctorId = dto.DoctorId,
        //        CreatedAt = DateTime.UtcNow,
        //        Items = dto.Items.Select(i => new PrescriptionItem
        //        {
        //            Medicine = i.Medicine,
        //            Dosage = i.Dosage,
        //            Duration = i.Duration,
        //            Notes = i.Notes
        //        }).ToList()
        //    };

        //    // save first to get Id
        //    var created = await _repo.CreateAsync(entity);

        //    // generate PDF
        //    var pdfRelativePath = await GeneratePdfAsync(created, dto, request);
        //    created.PdfPath = pdfRelativePath;

        //    // persist PdfPath
        //    await _repo.CreateAsync(created); // naive way: we saved previously; better to update instead
        //    // To avoid duplicate, update using DB context directly:
        //    // but for simplicity, load from repo, set PdfPath and SaveChanges via repository:
        //    // (We will modify repository to support update OR directly via EF as below.)

        //    // NOTE: better approach: update the record
        //    // For clean code, update PdfPath using reflection to DbContext:
        //    // (Assuming repository.CreateAsync returned tracked entity.)
        //    return ToResponse(created, request);
        //}
        public async Task<PrescriptionResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto dto, HttpRequest request)
        {
            // 1. Map DTO → Entity
            var entity = new Prescription
            {
                AppointmentId = dto.AppointmentId,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                CreatedAt = DateTime.UtcNow,
                Items = dto.Items.Select(i => new PrescriptionItem
                {
                    Medicine = i.Medicine,
                    Dosage = i.Dosage,
                    Duration = i.Duration,
                    Notes = i.Notes
                }).ToList()
            };

            // 2. Generate PDF BEFORE saving to DB
            var pdfRelativePath = await GeneratePdfAsync(entity, dto, request);

            // Assign generated PDF path (this MUST NOT be null)
            entity.PdfPath = pdfRelativePath;

            // 3. Save ONLY ONCE — with PdfPath included
            await _repo.CreateAsync(entity);

            // 4. Build Response DTO
            return new PrescriptionResponseDto
            {
                PrescriptionId = entity.PrescriptionId,
                PdfUrl = $"{request.Scheme}://{request.Host}/{pdfRelativePath}"
                //Message = "Prescription created successfully"
            };
        }


        public async Task<PrescriptionResponseDto> GetByIdAsync(int id, HttpRequest request)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;
            return ToResponse(p, request);
        }

        public async Task<IEnumerable<PrescriptionResponseDto>> GetByPatientAsync(int patientId, HttpRequest request)
        {
            var list = await _repo.GetByPatientAsync(patientId);
            return list.Select(p => ToResponse(p, request));
        }

        public async Task<IEnumerable<PrescriptionResponseDto>> GetByDoctorAsync(int doctorId, HttpRequest request)
        {
            var list = await _repo.GetByDoctorAsync(doctorId);
            return list.Select(p => ToResponse(p, request));
        }

        private PrescriptionResponseDto ToResponse(Prescription p, HttpRequest req)
        {
            var baseUrl = $"{req.Scheme}://{req.Host}";
            return new PrescriptionResponseDto
            {
                PrescriptionId = p.PrescriptionId,
                AppointmentId = p.AppointmentId,
                PatientId = p.PatientId,
                DoctorId = p.DoctorId,
                CreatedAt = p.CreatedAt,
                PdfUrl = string.IsNullOrEmpty(p.PdfPath) ? null : $"{baseUrl}/{p.PdfPath}",
                Items = p.Items.Select(i => new PrescriptionItemDto
                {
                    Medicine = i.Medicine,
                    Dosage = i.Dosage,
                    Duration = i.Duration,
                    Notes = i.Notes
                }).ToList()
            };
        }

        private async Task<string> GeneratePdfAsync(Prescription created, PrescriptionCreateDto dto, HttpRequest req)
        {
            // ensure folder
            var folder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "prescriptions");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var filename = $"prescription_{created.PrescriptionId}_{Guid.NewGuid()}.pdf";
            var filePath = Path.Combine(folder, filename);

            // Build PDF using QuestPDF
            var baseUrl = $"{req.Scheme}://{req.Host}";
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Row(row =>
                    {
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("E-Prescription").FontSize(18).Bold();
                            col.Item().Text($"Prescription ID: {created.PrescriptionId}");
                            col.Item().Text($"Date: {created.CreatedAt:yyyy-MM-dd HH:mm}");
                        });
                        row.ConstantColumn(100).AlignRight().Text("Clinic/Doctor").FontSize(12);
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text($"Patient ID: {created.PatientId}");
                        col.Item().Text($"Doctor ID: {created.DoctorId}");
                        col.Item().Text($"Appointment ID: {created.AppointmentId}");
                        col.Item().Text(" ");
                        col.Item().Element(e =>
                        {
                            e.Column(c =>
                            {
                                c.Item().Text("Medicines:").Bold();
                                foreach (var it in created.Items)
                                {
                                    c.Item().PaddingVertical(3).Text($"{it.Medicine} — {it.Dosage} — {it.Duration}").FontSize(11);
                                    if (!string.IsNullOrWhiteSpace(it.Notes))
                                        c.Item().Text($"Notes: {it.Notes}").FontSize(10).Italic();
                                }
                            });
                        });
                        if (!string.IsNullOrWhiteSpace(dto.Notes))
                            col.Item().Text($"Doctor notes: {dto.Notes}").Italic();
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated by ODC System");
                    });
                });
            });

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                doc.GeneratePdf(fs);
            }

            // return relative path for DB storage
            var relative = Path.Combine("prescriptions", filename).Replace("\\", "/");
            // assign to created entity
            created.PdfPath = relative;

            // update via repository: simpler to call EF directly (but we avoid exposing DbContext).
            // The repository can be extended to update; for now, call repository.CreateAsync returned a tracked entity, so SaveChanges already happened.
            // To persist PdfPath we should update via context here (using repo pattern, you can add UpdatePdfPath method).
            // We'll attempt to update entity via repository pattern by re-saving:
            return relative;
        }
        public async Task<IEnumerable<PrescriptionResponseDto>> GetByPatientId(int patientId)
        {
            var list = await _repo.GetByPatientIdAsync(patientId);
            return list.Select(p => ToResponse(p, null));  // return DTO with PdfPath
        }

    }
}
