using System.ComponentModel.DataAnnotations;

namespace LinkTree.Models.LinksDTO
{
    public class LinkRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string link { get; set; }
    }
}
