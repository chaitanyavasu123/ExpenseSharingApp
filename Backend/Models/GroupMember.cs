﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GroupMember
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
