﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOSPITAL2_LAB1.Data;
using HOSPITAL2_LAB1.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Numerics;
using System.Composition;

namespace HOSPITAL2_LAB1.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorsController : Controller
    {
        private readonly HOSPITAL2Context _context;

        public DoctorsController(HOSPITAL2Context context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> WaitingForApproval()
        {
            return View();
        }


        // GET: Doctors/Details

        public async Task<IActionResult> Details()
        {
            // Get the currently logged-in user's ID
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the doctor's information from the database
            var doctor = await _context.Doctors
                .Include(d => d.SpecializationNavigation)
                .FirstOrDefaultAsync(m => m.UserId == loggedInUserId);

            if (doctor == null)
            {
                return View("WaitingForApproval");
            }

            // Pass the doctor's information to the view
            return View(doctor);
        }



        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["Specialization"] = new SelectList(_context.Specializations, "SpecializationId", "SpecializationId");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Doctors/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,Name,Surname,Email,Education,PhotoUrl,Specialization,UserId")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Specialization"] = new SelectList(_context.Specializations, "SpecializationId", "SpecializationId", doctor.Specialization);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["Specialization"] = new SelectList(_context.Specializations, "SpecializationId", "SpecializationId", doctor.Specialization);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,Name,Surname,Email,Education,PhotoUrl,Specialization,UserId")] Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Specialization"] = new SelectList(_context.Specializations, "SpecializationId", "SpecializationId", doctor.Specialization);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.SpecializationNavigation)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Doctors == null)
            {
                return Problem("Entity set 'HOSPITAL2Context.Doctors'  is null.");
            }
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return (_context.Doctors?.Any(e => e.DoctorId == id)).GetValueOrDefault();
        }


        //crudi per raporte

        public async Task<IActionResult> CreateRaport()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (doctor != null)
            {
                // Get the list of patients who have reservations with the current doctor
                var patientsWithReservations = await _context.Reservations
                    .Where(reservation => reservation.Doctor == doctor.DoctorId)
                    .Select(reservation => reservation.PatientNavigation) // Use PatientNavigation to get the patient details
                    .Distinct()
                    .ToListAsync();

                // Create a SelectList with filtered patients
                ViewBag.PatientList = new SelectList(patientsWithReservations, "PatientId", "FullName");
            }
            else
            {
                ModelState.AddModelError("", "You have to provide personal info first.");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRaport([Bind("ReportID,ReportType,ReportDate,ReportDescription, Patient")] Report report)
        {

            if (string.IsNullOrWhiteSpace(report.ReportType))
            {
                ModelState.AddModelError("ReportType", "Report Type is required.");
            }
            if (report.ReportDate==null)
            {
                ModelState.AddModelError("ReportDate", "Date is required.");
            }
            if (string.IsNullOrWhiteSpace(report.ReportDescription))
            {
                ModelState.AddModelError("ReportDescription", "Description is required.");
            }

            if (report.Patient == null)
            {
                ModelState.AddModelError("Patient", "Patient is required.");
            }

            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                
                var doctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

                if (doctor != null)
                {
                    var existingRaport = await _context.Reports
                        .FirstOrDefaultAsync(a => a.ReportType == report.ReportType &&
                                                  a.ReportDate == report.ReportDate &&
                                                  a.ReportDescription == report.ReportDescription &&
                                                  a.Patient == report.Patient);

                    if (existingRaport != null)
                    {
                        ModelState.AddModelError("", "The report is not available! ");
                    }
                    else
                    {
                        report.Doctor = doctor.DoctorId;
                        _context.Add(report);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "You have to provide personal info first.");
                }


            }

            // Get the list of patients who have reservations with the current doctor
            var currentDoctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);
            var patientsWithReservations = await _context.Reservations
                .Where(reservation => reservation.Doctor == currentDoctor.DoctorId)
                .Select(reservation => reservation.PatientNavigation) // Use PatientNavigation to get the patient details
                .Distinct()
                .ToListAsync();

            // Create a SelectList with filtered patients
            ViewBag.PatientList = new SelectList(patientsWithReservations, "PatientId", "FullName");

            return View(report);
        }

        public async Task<IActionResult> Reports()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (doctor != null)
            {
                var report = await _context.Reports
                    .Include(a => a.PatientNavigation)
                    .Where(a => a.Doctor == doctor.DoctorId)
                    .ToListAsync();

                return View(report);
            }

            // Handle case where the patient is not found
            return NotFound();
        }



        //edit


        public async Task<IActionResult> EditReport(int? id)
        {
            if (id == null || _context.Reports == null)
            {
                return NotFound();
            }



            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);
            if(doctor != null) { 
                var patientsWithReservations = await _context.Reservations
                   .Where(reservation => reservation.Doctor == doctor.DoctorId)
                   .Select(reservation => reservation.PatientNavigation) // Use PatientNavigation to get the patient details
                   .Distinct()
                   .ToListAsync();
                ViewBag.PatientList = new SelectList(patientsWithReservations, "PatientId", "FullName");

            }
            else
            {
                ModelState.AddModelError("", "You have to provide personal info first.");

            }

            return View(report);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReport(int id, [Bind("ReportID,ReportType,ReportDate,ReportDescription, Patient")] Report editedReport)
        {

            if (string.IsNullOrWhiteSpace(editedReport.ReportType))
            {
                ModelState.AddModelError("ReportType", "Report Type is required.");
            }
            if (editedReport.ReportDate == null)
            {
                ModelState.AddModelError("ReportDate", "Date is required.");
            }
            if (string.IsNullOrWhiteSpace(editedReport.ReportDescription))
            {
                ModelState.AddModelError("ReportDescription", "Description is required.");
            }
            if (editedReport.Patient == null)
            {
                ModelState.AddModelError("Patient", "Patient is required.");
            }

            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {

                try
                {
                    var existingReport = await _context.Reports.FindAsync(id);

                    existingReport.ReportType = editedReport.ReportType;
                    existingReport.ReportDate = editedReport.ReportDate;
                    existingReport.ReportDescription = editedReport.ReportDescription;
                    existingReport.Patient = editedReport.Patient;
                    _context.Update(existingReport);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Reports));

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(editedReport.ReportId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var currentDoctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);
            var patientsWithReservations = await _context.Reservations
                .Where(reservation => reservation.Doctor == currentDoctor.DoctorId)
                .Select(reservation => reservation.PatientNavigation) // Use PatientNavigation to get the patient details
                .Distinct()
                .ToListAsync();

            // Create a SelectList with filtered patients
            ViewBag.PatientList = new SelectList(patientsWithReservations, "PatientId", "FullName");
            return View(editedReport);
        }
        //
        private bool ReportExists(int id)
        {
            return (_context.Reports?.Any(e => e.ReportId == id)).GetValueOrDefault();
        }

        private bool IsDuplicateReports(Report editedReport)
        {
            return _context.Reports.Any(r =>
               r.ReportId != editedReport.ReportId &&
               r.ReportType == editedReport.ReportType &&
               r.ReportDate == editedReport.ReportDate &&
               r.ReportDescription == editedReport.ReportDescription &&
               r.Patient == editedReport.Patient
           );
        }

        //delete

        public async Task<IActionResult> DeleteReport(int? id)
        {
            if (id == null || _context.Reports == null)
            {
                return NotFound();
            }

            var report = await _context.Reports

                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            ViewBag.PatientList = new SelectList(_context.Patients, "PatientId", "FullName");
            return View(report);

        }

        [HttpPost, ActionName("DeleteReport")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedReport(int id)
        {
            if (_context.Reports == null)
            {
                return Problem("Entity set 'HOSPITAL2Context.Reports'  is null.");
            }
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
            }

            await _context.SaveChangesAsync();

            ViewBag.PatientList = new SelectList(_context.Patients, "PatientId", "FullName");

            return RedirectToAction(nameof(Index));
        }


        // mbarimi crudit per raporte 


        //shfaqja e termineve nga pacienti tek doktori
        public async Task<IActionResult> Appointments()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (doctor != null)
            {
                var reservation = await _context.Reservations
                    .Include(a => a.PatientNavigation)
                    .Where(a => a.Doctor == doctor.DoctorId)
                    .ToListAsync();

                return View(reservation);
            }

            //case where the patient is not found
            return NotFound();
        }
        public async Task<IActionResult> Patients()
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(a => a.UserId == loggedInUserId);

            if (doctor != null)
            {
                // Get the patients who have reservations with the specific doctor
                var patients = await _context.Patients
                    .Where(p => p.Reservations.Any(r => r.DoctorNavigation.DoctorId == doctor.DoctorId))
                    .ToListAsync();

                return View(patients);
            }

            return NotFound();
        }



    }
}



