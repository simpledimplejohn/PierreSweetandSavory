using System.Collections.Generic;

namespace Bakery.Models
{
  public class Treat
  {
    public Treat()
    {
      this.JoinEntities = new HashSet<FlavorTreat>();
    }

    public int TreatId { get; set; }
    public string Description { get; set; }
    public virtual ApplicationUser User { get; set; } //this allows Idenity to lock out this class--also in the TreatController

    public virtual ICollection<FlavorTreat> JoinEntities { get;}
  }
}