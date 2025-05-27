using System;

namespace SEP4_User_Service.Application.Exceptions
{
    // Exception der kastes, når en bruger forsøges oprettet med en email, der allerede eksisterer.
    public class UserAlreadyExistsException : Exception
    {
        // Initialiserer med en specifik fejlbesked
        public UserAlreadyExistsException(string message)
            : base(message) { }

        // Initialiserer med en standardbesked
        public UserAlreadyExistsException()
            : base("Bruger med denne email findes allerede.") { }

        // Initialiserer med fejlbesked og en inner exception
        public UserAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
