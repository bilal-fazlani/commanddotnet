namespace CommandDotNet.Execution
{
    public enum MiddlewareStages
    {
        PreTransformInput,
        TransformInput,
        PostTransformInputPreBuild,
        Build,
        PostBuildPreParseInput,
        ParseInput,
        PostParseInputPreBindValues,
        BindValues,
        PostBindValuesPreInvoke,
        Invoke
    }
}