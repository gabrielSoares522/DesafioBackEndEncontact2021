using System.ComponentModel.DataAnnotations;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Controllers.Models
{
    public class SaveContactRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ContactBookId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(11)]
        public string Phone { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(50)]
        public string Address { get; set; }

        public IContact ToContact() => new Contact(Id,ContactBookId,CompanyId, Name,Phone,Email,Address);
    }
}
