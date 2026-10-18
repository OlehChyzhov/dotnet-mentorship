using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Application.Querying;
using Airbnb.Contracts.HTTP;
using Airbnb.Contracts.HTTP.Paging;
using MapsterMapper;

namespace Airbnb.Audit.Application.Services;

public class BookingAuditService : IBookingAuditService
{
    private readonly IBookingAuditRepository _auditRepository;
    private readonly IMapper _mapper;
    
    public BookingAuditService(IBookingAuditRepository auditRepository, IMapper mapper)
    {
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<Result<BookingAuditDto>> GetAuditByIdAsync(string id)
    {
        var audit = await _auditRepository.GetByIdAsync(id);
        if (!audit.IsSuccessful)
        {
            return $"Failed to retrieve an audit with id '{id}': {audit.Message}";
        }

        var auditDto = _mapper.Map<BookingAuditDto>(audit.Value!);
        return auditDto;
    }

    public async  Task<Result<PagedResult<BookingAuditDto>>> GetAuditsPaged(BookingAuditFilter parameters)
    {
        var auditsPaged = await _auditRepository.GetBookingsPagedAsync(parameters);
        var auditsDto = _mapper.Map<List<BookingAuditDto>>(auditsPaged.Items);
        
        var auditResult = new PagedResult<BookingAuditDto>(
            items: auditsDto, 
            totalCount: auditsPaged.Metadata.TotalCount, 
            pageNumber: auditsPaged.Metadata.CurrentPage, 
            pageSize: auditsPaged.Metadata.PageSize);

        return auditResult;
    }
}