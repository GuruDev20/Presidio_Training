using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    using LibraryManagementSystem.Interfaces;
    using LibraryManagementSystem.Models;

    public class MemberRepository : IMemberRepository
    {
        private readonly List<Member> members = new();
        private int idCounter = 1001;

        public void Add(Member member)
        {
            member.Id = idCounter++;
            members.Add(member);
        }

        public Member GetById(int id) => members.FirstOrDefault(m => m.Id == id);

        public IEnumerable<Member> GetAll() => members;
    }
}
