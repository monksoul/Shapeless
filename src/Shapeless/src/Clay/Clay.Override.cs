// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Shapeless;

/// <summary>
///     流变对象
/// </summary>
public partial class Clay
{
    /// <summary>
    ///     获取 <see cref="InvokeMemberBinder" /> 类型的 <c>TypeArguments</c> 属性访问器
    /// </summary>
    /// <remarks>实际上获取的是内部类型 <c>CSharpInvokeMemberBinder</c> 的 <c>TypeArguments</c> 属性访问器。</remarks>
    internal static readonly Lazy<Func<object, object?>> _getCSharpInvokeMemberBinderTypeArguments = new(() =>
    {
        // 获取内部的 CSharpInvokeMemberBinder 类型
        var csharpInvokeMemberBinderType =
            typeof(Binder).Assembly.GetType("Microsoft.CSharp.RuntimeBinder.CSharpInvokeMemberBinder")!;

        // 获取 TypeArguments 属性对象
        var typeArgumentsProperty =
            csharpInvokeMemberBinderType.GetProperty("TypeArguments", BindingFlags.Public | BindingFlags.Instance)!;

        // 创建 TypeArguments 属性访问器
        return csharpInvokeMemberBinderType.CreatePropertyGetter(typeArgumentsProperty);
    });

    /// <inheritdoc />
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = GetValue(binder.Name);
        return true;
    }

    /// <inheritdoc />
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        SetValue(binder.Name, value);
        return true;
    }

    /// <inheritdoc />
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        result = GetValue(indexes[0]);
        return true;
    }

    /// <inheritdoc />
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        SetValue(indexes[0], value);
        return true;
    }

    /// <inheritdoc />
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        // 处理 Clay 实例作为方法调用情况
        switch (args)
        {
            // 处理没有提供参数情况
            case { Length: 0 }:
                result = ToJsonString();
                return true;
            // 处理单个参数情况
            case { Length: 1 }:
                switch (args)
                {
                    // 处理 clay(JsonSerializerOptions) 情况
                    case [JsonSerializerOptions jsonSerializerOptions]:
                        result = ToJsonString(jsonSerializerOptions);
                        return true;
                    // 处理 clay(identifier) 情况
                    case [string or char or int or Index or Range]:
                        result = GetValue(args[0]!);
                        return true;
                    // 处理 clay(Type) 情况
                    case [Type resultType]:
                        result = As(resultType);
                        return true;
                    // 处理 clay(ClayOptions) 情况
                    case [ClayOptions clayOptions]:
                        result = Rebuilt(clayOptions);
                        return true;
                }

                break;
            // 处理两个参数情况
            case { Length: 2 }:
                switch (args)
                {
                    // 处理 clay(Type, JsonSerializerOptions) 情况
                    case [Type resultType, JsonSerializerOptions jsonSerializerOptions]:
                        result = As(resultType, jsonSerializerOptions);
                        return true;
                }

                break;
        }

        return base.TryInvoke(binder, args, out result);
    }

    /// <inheritdoc />
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        // 获取标识符
        var identifier = binder.Name;

        // 检查该成员是否是一个委托
        if (DelegateMap.TryGetValue(identifier, out var @delegate))
        {
            return DynamicInvokeDelegate(@delegate, args, out result);
        }

        // 获取调用方法的泛型参数数组
        var typeArguments = _getCSharpInvokeMemberBinderTypeArguments.Value.Invoke(binder) as Type[];

        // 处理类型转换操作
        switch (typeArguments)
        {
            // 处理空泛型参数情况
            case { Length: 0 }:
                switch (args)
                {
                    // 处理 clay.Prop() 情况
                    case { Length: 0 }:
                        result = Contains(identifier);
                        return true;
                    // 处理 clay.Prop(Type) 情况
                    case [Type resultType]:
                        result = Get(identifier, resultType);
                        return true;
                    // 处理 clay.Prop(Type, JsonSerializerOptions) 情况
                    case [Type resultType, JsonSerializerOptions jsonSerializerOptions]:
                        result = Get(identifier, resultType, jsonSerializerOptions);
                        return true;
                    // 处理 clay.Prop(Type, null) 情况
                    case [Type resultType, null]:
                        result = Get(identifier, resultType);
                        return true;
                    // 处理 clay.Prop(Func<string?, object?>) 情况
                    case [Func<string?, object?> converter]:
                        result = converter(FindNode(identifier).As<string>());
                        return true;
                }

                break;
            // 处理单个泛型参数情况
            case { Length: 1 }:
                switch (args)
                {
                    // 处理 clay.Prop<T>() 情况
                    case { Length: 0 }:
                        result = Get(identifier, typeArguments[0]);
                        return true;
                    // 处理 clay.Prop<T>(JsonSerializerOptions) 情况
                    case [JsonSerializerOptions jsonSerializerOptions]:
                        result = Get(identifier, typeArguments[0], jsonSerializerOptions);
                        return true;
                    // 处理 clay.Prop<T>(null) 情况
                    case [null]:
                        result = Get(identifier, typeArguments[0]);
                        return true;
                }

                break;
        }

        return base.TryInvokeMember(binder, args, out result);
    }

    /// <inheritdoc />
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        // 转换为目标类型
        result = As(binder.Type, Options.JsonSerializerOptions);

        // 检查是否启用转换后执行模型验证
        if (result is not null && Options.ValidateAfterConversion)
        {
            Validator.ValidateObject(result, new ValidationContext(result), true);
        }

        return true;
    }

    /// <summary>
    ///     动态调用委托
    /// </summary>
    /// <remarks>在第一个参数是 ClayContext 类型时自动创建并传递 ClayContext 实例。</remarks>
    /// <param name="delegate">
    ///     <see cref="Delegate" />
    /// </param>
    /// <param name="args">委托实参数组</param>
    /// <param name="result">返回结果</param>
    internal bool DynamicInvokeDelegate(Delegate? @delegate, object?[]? args, out object? result)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(@delegate);

        // 获取调用委托参数定义数组
        var parameters = @delegate.GetType().GetMethod("Invoke")?.GetParameters();

        // 准备实参列表
        var invokeArgs = parameters is { Length: > 0 } && parameters[0].ParameterType == typeof(ClayContext)
            ? new object[] { new ClayContext(this) }.Concat(args ?? []).ToArray()
            : args;

        result = @delegate.DynamicInvoke(invokeArgs);

        return true;
    }
}