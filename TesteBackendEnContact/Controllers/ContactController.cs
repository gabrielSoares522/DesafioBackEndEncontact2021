using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using CsvHelper;

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
            if (file.FileName.EndsWith(".csv"))
            {
                List<Contact> contacts = new List<Contact>();
                using (var sreader = new StreamReader(file.OpenReadStream()))
                {
                    string[] headers = sreader.ReadLine().Split(',');
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(',');
                        int Id = int.Parse(rows[0].ToString());
                        int ContactBookId = int.Parse(rows[1].ToString());
                        int CompanyId = int.Parse(rows[2].ToString());
                        string Name = rows[3].ToString();
                        string Phone = rows[4].ToString();
                        string Email = rows[5].ToString();
                        string Address = rows[6].ToString();

                        Contact contact = new Contact(Id,ContactBookId,CompanyId,Name,Phone,Email,Address);
                        contacts.Add(contact);
                        await ContactRepository.SaveAsync(contact);
                    }
                }
            }
            else
            {
                return BadRequest("the api only processes .csv files");  
            }
            return Ok();
                //ContactRepository.GetAsync(id);
        }
    }
}
