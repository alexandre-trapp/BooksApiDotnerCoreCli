using System;
using System.Collections.Generic;

using BooksApi.Models;
using BooksApi.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace BooksApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {
        private readonly BookService _bookService;
        private ILogger<BooksController> _logger;
        public BooksController(BookService bookService, ILogger<BooksController> logger) {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<Book>> Get() =>
            _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public ActionResult<Book> Get(string id) {
            var book = _bookService.Get(id);

            if (book == null) {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public ActionResult<Book> Create(Book book) {

            try {

                _bookService.Create(book);
                SendMessage(book);

                return CreatedAtRoute("GetBook", new { id = book.Id.ToString() }, book);
            }
            catch (Exception e) {

                _logger.LogError("Erro ao tentar cadastrar um livro", e);
                return new StatusCodeResult(500);
            }
        }

        private void SendMessage(Book book) {
            
            SendBookMessageBroker.Send(book, _logger);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Book bookIn) {
            var book = _bookService.Get(id);

            if (book == null) {
                return NotFound();
            }

            _bookService.Update(id, bookIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id) {
            var book = _bookService.Get(id);

            if (book == null) {
                return NotFound();
            }

            _bookService.Remove(book.Id);

            return NoContent();
        }
    }
}