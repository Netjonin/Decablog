using AutoMapper;
using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Models;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DecaBlog.Services.Implementations
{
    public class StackService : IStackService
    {
        private readonly IStackRepository _stackRepository;
        private readonly IMapper _mapper;
        public StackService(IStackRepository stackRepository, IMapper mapper)
        {
            _stackRepository = stackRepository;
            _mapper = mapper;
        }

        public StackMinInfoToReturnDto AddStack(StackToAddDto model)
        {
            var stack = _mapper.Map<Stack>(model);
            var stackToReturn = new StackMinInfoToReturnDto();
            var added = _stackRepository.AddStack(stack);
            if (added > 0)
                stackToReturn = _mapper.Map<StackMinInfoToReturnDto>(stack);
            return stackToReturn;
        }

        public Stack GetStackByName(string name) =>
            _stackRepository.GetStackByName(name);

        public async Task<IEnumerable<StackMinInfoToReturnDto>> GetAllStacks()
        {
            var stacks = await _stackRepository.GetAllStacks(); //changed _repository to _stackRepository
            if (stacks == null)
                return null;
            var data = _mapper.Map<IEnumerable<StackMinInfoToReturnDto>>(stacks);
            return data;
        }

        public async Task<StackMinInfoToReturnDto> GetStackById(string Id)
        {
            var stack = await _stackRepository.GetStackById(Id);
            return _mapper.Map<StackMinInfoToReturnDto>(stack);
        }
    }
}
