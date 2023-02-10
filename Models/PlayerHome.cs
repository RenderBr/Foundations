using Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundations.Models
{
    public class PlayerHome : BsonModel
    {
        private string _name = String.Empty;
        public string Name
        {
            get
              => _name;
            set
            {
                _ = this.SaveAsync(x => x.Name, value);
                _name = value;
            }
        }

        private string _accountName = String.Empty;
        public string AccountName
        {
            
            get
                => _accountName;
            
            set
            {
                    _ = this.SaveAsync(x => x.AccountName, value);
                    _accountName = value;
                }
            }

        private int _x;
        public int X
        {
            get
              => _x;
            set
            {
                _ = this.SaveAsync(x => x.X, value);
                _x = value;
            }
        }

        private int _y;
        public int Y
        {
            get
              => _y;
            set
            {
                _ = this.SaveAsync(x => x.Y, value);
                _y = value;
            }
        }

        private int _z;
        public int Z
        {
            get
              => _z;
            set
            {
                _ = this.SaveAsync(x => x.Z, value);
                _z = value;
            }
        }
        private int _worldId;
        public int WorldId
        {
            get
              => _worldId;
            set
            {
                _ = this.SaveAsync(x => x.WorldId, value);
                _worldId = value;
            }
        }

    }
}
