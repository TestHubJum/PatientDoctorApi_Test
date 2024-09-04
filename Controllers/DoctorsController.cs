using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientDoctorApi.Models;
using PatientDoctorApi.Models.ViewModels;

namespace PatientDoctorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public DoctorsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorListView>>> GetDoctors(
            string sortBy = "FullName", int page = 1, int pageSize = 10)
        {
            var query = _context.Doctors
                .Include(d => d.Office)
                .Include(d => d.Specialization)
                .Include(d => d.Section)
                .AsQueryable();

            query = sortBy switch
            {
                "SpecializatiomName" => query.OrderBy(d => d.Specialization.Name),
                "OfficeNumber" => query.OrderBy(d => d.Office.Number),
                _ => query.OrderBy(d => d.FullName),
            };

            var doctors = await query
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .Select(d => new DoctorListView
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    OfficeNumber = d.Office.Number,
                    SpecializationName = d.Specialization.Name,
                    SectionNumber = d.Section.Number
                }).ToListAsync();

            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorEditView>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            var doctorEditView = new DoctorEditView
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                OfficeId = doctor.OfficeId,
                SpecializationId = doctor.SpecializationId,
                SectionId = doctor.SectionId,
            };

            return Ok(doctorEditView);
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(DoctorEditView doctorEditView)
        {
            var doctor = new Doctor
            {
                FullName = doctorEditView.FullName,
                OfficeId = doctorEditView.OfficeId,
                SpecializationId = doctorEditView.SpecializationId,
                SectionId = doctorEditView.SectionId,
            };

            try
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
            }
            catch (Exception e) {
                await Console.Out.WriteLineAsync(e.ToString());
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, DoctorEditView doctorEditView)
        {
            if (id != doctorEditView.Id) return BadRequest();

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            try
            {
                doctor.FullName = doctorEditView.FullName;
                doctor.OfficeId = doctorEditView.OfficeId;
                doctor.SpecializationId = doctorEditView.SpecializationId;
                doctor.SectionId = doctorEditView.SectionId;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            try
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                return BadRequest();
            }
        }
    }
}
