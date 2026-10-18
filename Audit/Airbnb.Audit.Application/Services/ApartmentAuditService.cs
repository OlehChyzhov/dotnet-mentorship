using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Application.Querying;
using Airbnb.Contracts.HTTP;
using Airbnb.Contracts.HTTP.Paging;
using MapsterMapper;

namespace Airbnb.Audit.Application.Services;

public class ApartmentAuditService : IApartmentAuditService
{
    private readonly IApartmentAuditRepository _auditRepository;
    private readonly IMapper _mapper;
    
    public ApartmentAuditService(IApartmentAuditRepository auditRepository, IMapper mapper)
    {
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<Result<ApartmentAuditDto>> GetAuditByIdAsync(string id)
    {
        var audit = await _auditRepository.GetByIdAsync(id);
        if (!audit.IsSuccessful)
        {
            return $"Failed to retrieve an audit with id '{id}': {audit.Message}";
        }

        var auditDto = _mapper.Map<ApartmentAuditDto>(audit.Value!);
        return auditDto;
    }

    public async Task<Result<PagedResult<ApartmentAuditDto>>> GetAuditsPaged(ApartmentAuditFilter parameters)
    {
        var auditsPaged = await _auditRepository.GetApartmentsPagedAsync(parameters);
        var auditsDto = _mapper.Map<List<ApartmentAuditDto>>(auditsPaged.Items);
        
        var auditResult = new PagedResult<ApartmentAuditDto>(
            items: auditsDto, 
            totalCount: auditsPaged.Metadata.TotalCount, 
            pageNumber: auditsPaged.Metadata.CurrentPage, 
            pageSize: auditsPaged.Metadata.PageSize);

        return auditResult;
    }
}