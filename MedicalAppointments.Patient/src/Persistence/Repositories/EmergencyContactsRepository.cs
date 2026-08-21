using Core.Interfaces.Repository;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class EmergencyContactsRepository : IEmergencyContactsRepository
    {
        private readonly AppDbContext _context;
        public EmergencyContactsRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EmergencyContacts> AddContact(EmergencyContacts emergency_Contacts)
        {
            _context.EmergencyContacts.Add(emergency_Contacts);
            await _context.SaveChangesAsync();
            return emergency_Contacts;
        }
        public async Task<EmergencyContacts> UpdateContact(EmergencyContacts emergency_Contacts)
        {
            _context.EmergencyContacts.Update(emergency_Contacts);
            await _context.SaveChangesAsync();
            return emergency_Contacts;
        }
        public async Task<bool> DeleteContact(EmergencyContacts emergency_Contacts)
        {
            _context.EmergencyContacts.Remove(emergency_Contacts);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteContact(int id)
        {
            var emergencyContact = await _context.EmergencyContacts.FindAsync(id);
            if (emergencyContact == null)
                return false;
            _context.EmergencyContacts.Remove(emergencyContact);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<EmergencyContacts>> GetContact()
        {
            return await _context.EmergencyContacts.ToListAsync();
        }

    }
}
