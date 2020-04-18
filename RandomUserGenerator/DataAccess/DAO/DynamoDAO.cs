using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomUserGenerator.DataAccess.DAO
{    
    public abstract class DynamoDAO
    {
        //As there's only one DAO, nothing can be abstracted into here yet.
        //Exists only for polymorphism on the ToDynamoRequest method so far.
    }
}
