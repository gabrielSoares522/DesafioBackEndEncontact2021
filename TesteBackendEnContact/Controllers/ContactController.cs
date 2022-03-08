using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<IContact>> Post(SaveContactRequest Contact, [FromServices] IContactRepository ContactRepository)
        {
            return Ok(await ContactRepository.SaveAsync(Contact.ToContact()));
        }

        [HttpDelete]
        public async Task Delete(int id, [FromServices] IContactRepository ContactRepository)
        {
            await ContactRepository.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<IEnumerable<IContact>> Get([FromServices] IContactRepository ContactRepository)
        {
            return await ContactRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<IContact> Get(int id, [FromServices] IContactRepository ContactRepository)
        {
            return await ContactRepository.GetAsync(id);
        }
    }
}
