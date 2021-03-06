﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NServiceBus.ObjectBuilder;

class UnitOfWorkValidatorTypeCache :
    IValidatorTypeCache
{
    ConcurrentDictionary<Type, ValidatorInfo> typeCache = new();

    static Type validatorType = typeof(IValidator<>);

    public bool TryGetValidators(Type messageType, IBuilder builder, out IEnumerable<IValidator> validators)
    {
        var validatorInfo = typeCache.GetOrAdd(messageType,
            type => new ValidatorInfo
            (
                validatorType: validatorType.MakeGenericType(type)
            ));

        if (validatorInfo.HasValidators.HasValue)
        {
            if (!validatorInfo.HasValidators.Value)
            {
                validators = Enumerable.Empty<IValidator>();
                return false;
            }
        }

        validators = builder
            .BuildAll(validatorInfo.ValidatorType)
            .Cast<IValidator>()
            .ToList();

        var any = validators.Any();
        validatorInfo.HasValidators = any;
        return any;
    }

    class ValidatorInfo
    {
        public ValidatorInfo(Type validatorType)
        {
            ValidatorType = validatorType;
        }
        public Type ValidatorType { get; }
        public bool? HasValidators;
    }
}