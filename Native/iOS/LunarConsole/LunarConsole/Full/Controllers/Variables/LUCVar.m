//
//  LUCVar.m
//
//  Lunar Unity Mobile Console
//  https://github.com/SpaceMadness/lunar-unity-console
//
//  Copyright 2018 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "LUCVar.h"

#import "Lunar-Full.h"

NSString * const LUCVarTypeNameBoolean = @"Boolean";
NSString * const LUCVarTypeNameInteger = @"Integer";
NSString * const LUCVarTypeNameFloat   = @"Float";
NSString * const LUCVarTypeNameString  = @"String";
NSString * const LUCVarTypeNameEnum    = @"Enum";
NSString * const LUCVarTypeNameUnknown = @"Unknown";

@interface LUCVar ()
{
    Class _cellClass;
	NSMutableArray<LUWeakReference *> * _observers;
}

@end

@implementation LUCVar

+ (instancetype)variableWithId:(int)entryId
                          name:(NSString *)name
                         value:(NSString *)value
                  defaultValue:(NSString *)defaultValue
                          type:(LUCVarType)type
                     cellClass:(Class)cellClass
{
    return [[self alloc] initWithId:entryId name:name value:value defaultValue:defaultValue type:type cellClass:cellClass];
}

- (instancetype)initWithId:(int)entryId name:(NSString *)name
                     value:(NSString *)value
              defaultValue:(NSString *)defaultValue
                      type:(LUCVarType)type
                 cellClass:(Class)cellClass
{
    self = [super initWithId:entryId name:name];
    if (self)
    {
        _value = value;
        _defaultValue = defaultValue;
        _cellClass = cellClass;
        _type = type;
        _range = LUMakeCVarRange(NAN, NAN);
		_observers = [NSMutableArray new];
    }
    return self;
}

#pragma mark -
#pragma mark Default value

- (void)resetToDefaultValue
{
    self.value = _defaultValue;
}

#pragma mark -
#pragma mark Flags

- (BOOL)hasFlag:(LUCVarFlags)flag
{
    return self.flags & flag;
}

#pragma mark -
#pragma mark Value Check

- (BOOL)isValidValue:(NSString *)value
{
	switch (_type) {
		case LUCVarTypeInteger:
			return LUStringTryParseInteger(value, NULL);
		case LUCVarTypeFloat:
			return LUStringTryParseFloat(value, NULL);
		default:
			return YES;
	}
}

#pragma mark -
#pragma mark UITableView

- (UITableViewCell *)tableView:(UITableView *)tableView cellAtIndexPath:(NSIndexPath *)indexPath
{
    NSString *identifier = NSStringFromClass(_cellClass);
    LUCVarTableViewCell *cell = (LUCVarTableViewCell *)[tableView dequeueReusableCellWithIdentifier:identifier];
    if (cell == nil)
    {
        cell = [[_cellClass alloc] initWithReuseIdentifier:identifier];
    }
    
    [cell setupVariable:self atIndexPath:indexPath];
    
    return cell;
}

#pragma mark -
#pragma mark Lookup 

+ (LUCVarType)typeForName:(NSString *)type
{
    if ([type isEqualToString:LUCVarTypeNameBoolean]) return LUCVarTypeBoolean;
    if ([type isEqualToString:LUCVarTypeNameInteger]) return LUCVarTypeInteger;
    if ([type isEqualToString:LUCVarTypeNameFloat])   return LUCVarTypeFloat;
    if ([type isEqualToString:LUCVarTypeNameString])  return LUCVarTypeString;
	if ([type isEqualToString:LUCVarTypeNameEnum])    return LUCVarTypeEnum;
    
    return LUCVarTypeUnknown;
}

+ (NSString *)typeNameForType:(LUCVarType)type
{
    switch (type)
    {
        case LUCVarTypeBoolean: return LUCVarTypeNameBoolean;
        case LUCVarTypeInteger: return LUCVarTypeNameInteger;
        case LUCVarTypeFloat:   return LUCVarTypeNameFloat;
        case LUCVarTypeString:  return LUCVarTypeNameString;
		case LUCVarTypeEnum:    return LUCVarTypeNameEnum;
        case LUCVarTypeUnknown: return LUCVarTypeNameUnknown;
    }
}

#pragma mark -
#pragma mark Observers

- (void)registerObserver:(id<LUCVarObserver>)observer
{
	for (LUWeakReference *reference in _observers)
	{
		id<LUCVarObserver> existingObserver = reference.target;
		if (observer == existingObserver)
		{
			return;
		}
	}
	
	[_observers addObject:[LUWeakReference referenceWithTarget:observer]];
	[observer cvarValueDidChange:self];
}

- (void)unregisterObserver:(id<LUCVarObserver>)observer
{
	for (LUWeakReference *reference in _observers)
	{
		id<LUCVarObserver> existingObserver = reference.target;
		if (observer == existingObserver)
		{
			[_observers removeObject:reference];
			return;
		}
	}
}

- (void)notifyObservers
{
	NSUInteger lostReferenceCount = 0;
	for (LUWeakReference *reference in _observers)
	{
		id<LUCVarObserver> observer = reference.target;
		if (observer == nil)
		{
			lostReferenceCount++;
			continue;
		}
		
		[observer cvarValueDidChange:self];
	}
	
	for (NSUInteger i = _observers.count; i >= 0 && lostReferenceCount > 0; --i)
	{
		if (_observers[i].isLost)
		{
			[_observers removeObjectAtIndex:i];
			lostReferenceCount--;
		}
	}
}

#pragma mark -
#pragma mark Properties

- (void)setValue:(NSString *)value
{
	_value = value;
	[self notifyObservers];
}

- (BOOL)isDefaultValue
{
    return [_value isEqualToString:_defaultValue];
}

- (BOOL)hasRange
{
    return !isnan(_range.min) && !isnan(_range.max);
}

@end
