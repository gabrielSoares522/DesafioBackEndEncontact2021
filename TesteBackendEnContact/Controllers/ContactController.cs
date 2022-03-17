using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Repository.Interface;
using System.Text.Json;

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

        [HttpPost]
        [Route("uploadCsv")]
        public async Task<IActionResult> UploadCsv(IFormFile file, [FromServices] IContactRepository ContactRepository)
        {
            List<Contact> contacts = new List<Contact>();
            if (file.FileName.EndsWith(".csv"))
            {
                using (var sreader = new StreamReader(file.OpenReadStream()))
                {
                    string[] headers = sreader.ReadLine().Split(',');
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(',');

                        int Id = 0, ContactBookId;
                        int? CompanyId;
                        string Name, Phone, Email, Address;

                        try
                        {
                            ContactBookId = int.Parse(rows[0].ToString());
                            CompanyId = int.Parse(rows[1].ToString());
                            Name = rows[2].ToString();
                            Phone = rows[3].ToString();
                            Email = rows[4].ToString();
                            Address = rows[5].ToString();
                        }
                        catch (Exception ex) {
                            continue;
                        }

                        if (ContactBookId == 0 || Name == null || Phone == null || Email == null || Address == null)
                        {
                            continue;
                        }

                        try
                        {
                            Contact contact = new Contact(Id, ContactBookId, CompanyId, Name, Phone, Email, Address);
                            contact = (Contact)await ContactRepository.SaveAsync(contact);
                            contacts.Add(contact);
                        } catch (Exception ex) { }
                    }
                }
            }
            else
            {
                return BadRequest("the api only processes .csv files");  
            }
            return Ok(JsonSerializer.Serialize(contacts));
        }

        [HttpGet]
        [Route("search/{keyWord}/{page}/{size}")]
        public async Task<IEnumerable<IContact>> Search(string keyWord,int page,int size,[FromServices] IContactRepository ContactRepository)
        {
            if (page < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page), "Invalid value of page");
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Invalid value of size");
            }

            return await ContactRepository.GetAsync(keyWord,page,size);
        }

        [HttpGet]
        [Route("searchByCompany/{companyId}")]
        public async Task<IEnumerable<IContact>> SearchByCompany(int companyId,[FromServices] IContactRepository ContactRepository)
        {
            return await ContactRepository.GetByCompanyAsync(companyId);
        }

    }
}
