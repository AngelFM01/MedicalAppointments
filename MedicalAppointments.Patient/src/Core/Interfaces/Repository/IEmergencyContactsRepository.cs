using Domain.Model;

namespace Core.Interfaces.Repository
{
    public interface IEmergencyContactsRepository
    {
        //CRUD
        Task<EmergencyContacts> AddContact(EmergencyContacts emergency_Contacts);
        Task<EmergencyContacts> UpdateContact(EmergencyContacts emergency_Contacts);
        Task<bool> DeleteContact(EmergencyContacts emergency_Contacts);
        Task<bool> DeleteContact(int id);
        Task<List<EmergencyContacts>> GetContact();
    }
}
