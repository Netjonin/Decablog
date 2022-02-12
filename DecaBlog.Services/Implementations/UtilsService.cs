using AutoMapper;
using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaBlog.Services.Implementations
{
    public class UtilsService : IUtilsService
    {
        private readonly IStackRepository _stackRepository;
        private readonly ISquadRepository _squadRepository;
        private readonly IMapper _mapper;
        public UtilsService(IStackRepository stackRepository, ISquadRepository squadRepository, IMapper mapper)
        {
            _stackRepository = stackRepository;
            _squadRepository = squadRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<StackMinInfoToReturnDto>, IEnumerable<SquadMinInfoToReturnDto>)> GetAllSquadsAndStack()
        {
            var allSquads = await _squadRepository.GetAllSquads();
            if (allSquads == null) return (null, null);
            var SquadDataToReturn = _mapper.Map<IEnumerable<SquadMinInfoToReturnDto>>(allSquads);
            var allStacks = await _stackRepository.GetAllStacks();
            if (allStacks == null) return (null, null);
            var StackDataToReturn = _mapper.Map<IEnumerable<StackMinInfoToReturnDto>>(allStacks);
            return (StackDataToReturn, SquadDataToReturn);
        }
    }
}
