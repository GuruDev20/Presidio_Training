using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    using LibraryManagementSystem.Interfaces;
    using LibraryManagementSystem.Models;

    public class MemberService
    {
        private readonly IMemberRepository _repository;
        private readonly ILogger _logger;
        public MemberService(IMemberRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void RegisterMember(Member member)
        {
            _repository.Add(member);
            _logger.Log($"Member registered: {member.Name}");
        }
    }
}
