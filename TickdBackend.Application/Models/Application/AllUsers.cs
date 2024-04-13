using System.ComponentModel.DataAnnotations.Schema;

namespace TickdBackend.Application.Models.Application
{
    public class AllUsers
    {
        public int AccountId { get; set; }
       
        public string FirstName { get; set; } =string.Empty;    
      
        public string LastName { get; set; } = string.Empty;    

    }
}
