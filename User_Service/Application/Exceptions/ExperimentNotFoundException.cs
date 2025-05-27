using System;

namespace SEP4_User_Service.Application.Exceptions
{
    // Exception der kastes, når et eksperiment ikke findes i databasen.
    public class ExperimentNotFoundException : Exception
    {
        public ExperimentNotFoundException(string message)
            : base(message) { }

        public ExperimentNotFoundException()
            : base("Eksperiment ikke fundet.") { }

        public ExperimentNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
