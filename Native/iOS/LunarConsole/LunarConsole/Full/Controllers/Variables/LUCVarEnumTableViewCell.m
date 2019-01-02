//
//  LUCVarEnumTableViewCell.m
//  LunarConsoleFull
//
//  Created by Alex Lementuev on 11/30/18.
//  Copyright © 2018 Space Madness. All rights reserved.
//

#import "LUCVarEnumTableViewCell.h"
#import "LUCVarTableViewCell+Inheritance.h"

#import "Lunar-Full.h"

@interface LUCVarEnumTableViewCell ()

@property (unsafe_unretained, nonatomic) IBOutlet UIButton *valueButton;

@end

@implementation LUCVarEnumTableViewCell

#pragma mark -
#pragma mark Variable

- (void)setupVariable:(LUCVar *)variable atIndexPath:(NSIndexPath *)indexPath
{
	[super setupVariable:variable atIndexPath:indexPath];
	
	LUTheme *theme = [LUTheme mainTheme];
	UIColor *titleColor = [variable hasFlag:LUCVarFlagsNoArchive] ? theme.variableVolatileTextColor : theme.variableTextColor;
	[_valueButton setTitleColor:titleColor forState:UIControlStateNormal];
	[_valueButton.titleLabel setFont:theme.actionsFont];
	
	LU_SET_ACCESSIBILITY_IDENTIFIER(_valueButton, @"Variable Enum Button");
}

- (void)updateUI
{
	[super updateUI];
	[_valueButton setTitle:self.variable.value forState:UIControlStateNormal];
}

#pragma mark -
#pragma mark Actions

- (IBAction)valueButtonPress:(id)sender
{
}

@end
