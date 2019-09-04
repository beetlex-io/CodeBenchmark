function Home() {
    this.url_ListExamples = '/ListExamples';
    this.url_DelRoundCase = '/DelRoundCase';
    this.url_ClearRoundCase = '/ClearRoundCase';
    this.url_AddRoundCase = '/AddRoundCase';
    this.url_GetRoundStatus = '/GetRoundStatus';
    this.url_GetAllStatus = '/GetAllStatus';
    this.url_GetStatus = '/GetStatus';
    this.url_Run = '/Run';
    this.url_GetLatency = '/GetLatency';
    this.url_Report = '/Report';
    this.url_AllRpsReport = '/AllRpsReport';
    this.url_Stop = '/Stop';
}
/**
* 'ListExamples(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.ListExamples = function (useHttp) {
    return api(this.url_ListExamples, {}, useHttp);
}
/**
* 'DelRoundCase(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.DelRoundCase = function (id, useHttp) {
    return api(this.url_DelRoundCase, { id: id }, useHttp);
}
/**
* 'ClearRoundCase(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.ClearRoundCase = function (useHttp) {
    return api(this.url_ClearRoundCase, {}, useHttp);
}
/**
* 'AddRoundCase(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.AddRoundCase = function (id, concurrent, secconds, useHttp) {
    return api(this.url_AddRoundCase, { id: id, concurrent: concurrent, secconds: secconds }, useHttp);
}
/**
* 'GetRoundStatus(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.GetRoundStatus = function (id, useHttp) {
    return api(this.url_GetRoundStatus, { id: id }, useHttp);
}
/**
* 'GetAllStatus(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.GetAllStatus = function (useHttp) {
    return api(this.url_GetAllStatus, {}, useHttp);
}
/**
* 'GetStatus(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.GetStatus = function (useHttp) {
    return api(this.url_GetStatus, {}, useHttp);
}
/**
* 'Run(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.Run = function (rounds, examples, useHttp) {
    return api(this.url_Run, { rounds: rounds, examples: examples }, useHttp, true);
}
/**
* 'GetLatency(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.GetLatency = function (roundid, id, useHttp) {
    return api(this.url_GetLatency, { roundid: roundid, id: id }, useHttp);
}
/**
* 'Report(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.Report = function (useHttp) {
    return api(this.url_Report, {}, useHttp);
}
/**
* 'AllRpsReport(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.AllRpsReport = function (useHttp) {
    return api(this.url_AllRpsReport, {}, useHttp);
}
/**
* 'Stop(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.Stop = function (useHttp) {
    return api(this.url_Stop, {}, useHttp);
}
