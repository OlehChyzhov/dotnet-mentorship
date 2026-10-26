using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Application.Querying;
using Airbnb.Shared.HTTP;
using MapsterMapper;

namespace Airbnb.Audit.Application.Services;

public class UserAuditService : IUserAuditService
{
    private readonly IUserAuditRepository _auditRepository;
    private readonly IMapper _mapper;
    
    public UserAuditService(IUserAuditRepository auditRepository, IMapper mapper)
    {
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserAuditDto>> GetAuditByIdAsync(string id)
    {
        var audit = await _auditRepository.GetByIdAsync(id);
        if (!audit.IsSuccessful)
        {
            return $"Failed to retrieve an audit with id '{id}': {audit.Message}";
        }

        var auditDto = _mapper.Map<UserAuditDto>(audit.Value!);
        return auditDto;
    }

    public async  Task<Result<PagedResult<UserAuditDto>>> GetAuditsPaged(UserAuditFilter parameters)
    {
        var auditsPaged = await _auditRepository.GetAllAuditsPaged(parameters);
        var auditsDto = _mapper.Map<List<UserAuditDto>>(auditsPaged.Items);
        
        var auditResult = new PagedResult<UserAuditDto>(
            items: auditsDto, 
            totalCount: auditsPaged.Metadata.TotalCount, 
            pageNumber: auditsPaged.Metadata.CurrentPage, 
            pageSize: auditsPaged.Metadata.PageSize);

        return auditResult;
    }
}