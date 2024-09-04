using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientDoctorApi.Models;
using PatientDoctorApi.Models.ViewModels;

namespace PatientDoctorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PatientsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListView>>> GetPatients(
            string sortBy = "LastName", int page = 1, int pageSize = 10)
        {
            var query = _context.Patients.Include(p => p.Section).AsQueryable();

            query = sortBy switch
            {
                "FirstName" => query.OrderBy(p => p.FirstName),
                "DateOfBirth" => query.OrderBy(p => p.DateOfBirth),
                _ => query.OrderBy(p => p.LastName),
            };

            var patients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PatientListView
                {
                    Id = p.Id,
                    FullName = $"{p.LastName} {p.FirstName} {p.MiddleName}",
                    Address = p.Address,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    SectionNumber = p.Section.Number

                }).ToListAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientEditView>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return BadRequest();

            var patientEditView = new PatientEditView
            {
                Id = patient.Id,
                LastName = patient.LastName,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                Address = patient.Address,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                SectionId = patient.SectionId,
            };
            return Ok(patientEditView);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(PatientEditView patientEditView)
        {
            var patient = new Patient
            {
                LastName = patientEditView.LastName,
                FirstName = patientEditView.FirstName,
                MiddleName = patientEditView.MiddleName,
                Address = patientEditView.Address,
                DateOfBirth = patientEditView.DateOfBirth,
                Gender = patientEditView.Gender,
                SectionId = patientEditView.SectionId
            };

            try
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientEditView patientEditView)
        {
            if (id != patientEditView.Id) return BadRequest();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            try
            {
                patient.LastName = patientEditView.LastName;
                patient.FirstName = patientEditView.FirstName;
                patient.MiddleName = patientEditView.MiddleName;
                patient.Address = patientEditView.Address;
                patient.DateOfBirth = patientEditView.DateOfBirth;
                patient.Gender = patientEditView.Gender;
                patient.SectionId = patientEditView.SectionId;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            try
            {
                _context.Patients.Remove(patient);
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
