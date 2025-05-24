using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Interfaces
{ 
    using LibraryManagementSystem.Models;

    public interface IMemberRepository
    {
        void Add(Member member);
        Member GetById(int id);
        IEnumerable<Member> GetAll();
    }
}
