function Home() {
    this.url_ListExamples = '/ListExamples';
    this.url_GetStatus = '/GetStatus';
    this.url_Run = '/Run';
    this.url_Report = '/Report';
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
Home.prototype.Run = function (concurrent, seconds, examples, useHttp) {
    return api(this.url_Run, { concurrent: concurrent, seconds: seconds, examples: examples }, useHttp);
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
* 'Stop(params).execute(function(result){});'
* 'FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
* 'https://github.com/IKende/FastHttpApi
**/
Home.prototype.Stop = function (useHttp) {
    return api(this.url_Stop, {}, useHttp);
}
