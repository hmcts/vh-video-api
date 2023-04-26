using System;

namespace VideoApi.Contract.Requests;

public class HearingDetailRequest
{
    public DateTimeOffset Time { get; set; }
    public string Judge { get; set; }
    public string CaseName { get; set; }
}
