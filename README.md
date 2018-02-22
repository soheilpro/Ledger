# Ledger
Ledger is an interactive CLI double-entry accounting application inspired by [ledger-cli.org](https://www.ledger-cli.org/).

## Download
+ Linux: [ledger-1.0-linux-x64.tar.gz](https://github.com/soheilpro/Ledger/releases/download/v1.0/ledger-1.0-linux-x64.tar.gz)
+ MacOS: [ledger-1.0-osx-x64.tar.gz](https://github.com/soheilpro/Ledger/releases/download/v1.0/ledger-1.0-osx-x64.tar.gz)
+ Windows: [ledger-1.0-win-x64.zip](https://github.com/soheilpro/Ledger/releases/download/v1.0/ledger-1.0-win-x64.zip)

## How It Works?
You use your editor of choice to enter transactions in a simple text file (called journal) and then run _Ledger_ to view different reports.

## Journal Format
A journal consists of a series of entries.

An entry begins with the keyword `@entry` and is followed by an index and a number of entry items. Each entry item has three parts: An account, an asset, and an amount.

```
@entry 2018-06-01 12:00
Assets:Bank EUR 5000
Equity:ProfitLoss:Income:Salary EUR -5000

@entry 2018-06-02 12:00
Equity:ProfitLoss:Expense:Food EUR 19.9
Equity:ProfitLoss:Expense:Tip EUR 2.98
Assets:Bank EUR -22.88
```

#### Index
* You can use anything such as dates or sequential numbers as index.
* Just be sure to format (and pad) them correctly as they are compared as strings.

#### Account
* Use colons (`:`) to create a hierarchy for your accounts.
* There are only two rules on how to structure your accounts:
  1. All accounts have to be under one of the three main accounts: `Assets`, `Liabilities`, or `Equity`.
  2. Income and expense accounts have to be under the `Equity:ProfitLoss:Income` and `Equity:ProfitLoss:Expense` accounts, respectively.

#### Asset
* An asset specifies a tangible entity (usually money) that you wish to keep track of.
* It can be anything from currencies (EUR, USD, etc.) to shares (AAPL, MSFT, etc.), to commodities. _Ledger_ doesn't really care.
* An account can hold many different assets.

#### Amount
* An amount can be any decimal number.
* A positive number means to debit the account, and a negative number means to credit the account.

## Usage
Run the application and pass the path of your journal file as the first argument:
```
$ ledger 2018.journal
```

Once inside the program, run any of the following commands or type `help` to view a list of all available commands.

#### View Balances
`balances` (or `b` for short) shows you a list of your accounts and their ending balances:

```
> balances

            Account             | Asset | Total Debit | Total Credit | Balance Debit | Balance Credit
------------------------------- | ----- | ----------- | ------------ | ------------- | --------------
Assets:Bank                     | EUR   |       5,000 |        22.88 |      4,977.12 |              0
Equity:ProfitLoss:Expense:Food  | EUR   |        19.9 |            0 |          19.9 |              0
Equity:ProfitLoss:Expense:Tip   | EUR   |        2.98 |            0 |          2.98 |              0
Equity:ProfitLoss:Income:Salary | EUR   |           0 |        5,000 |             0 |          5,000
```

#### View Your Profit and Loss
To see how much you have gained, or lost, run the `profitloss` (or `pl` for short) command:

```
> profitloss

     Account      | Asset | Balance Debit | % | Balance Credit |  %
----------------- | ----- | ------------- | - | -------------- | ---
Equity:ProfitLoss | EUR   |             0 | 0 |       4,977.12 | 100
```

To see a list of your top incomes or expenses, run:

```
profitloss Equity:ProfitLoss:Expense:*

            Account            | Asset | Balance Debit |   %   | Balance Credit | %
------------------------------ | ----- | ------------- | ----- | -------------- | -
Equity:ProfitLoss:Expense:Food | EUR   |          19.9 | 86.98 |              0 | 0
Equity:ProfitLoss:Expense:Tip  | EUR   |          2.98 | 13.02 |              0 | 0
```

#### Check Balances
To make sure that your balances satisfy the fundamental accounting equation (Assets = Liabilities + Equity), run the `balance-check` (or `bc` for short) command:

```
> balance-check

Asset |  Assets  | Liabilities + Equity | Liabilities |  Equity   | Diff
----- | -------- | -------------------- | ----------- | --------- | ----
EUR   | 4,977.12 |            -4,977.12 |           0 | -4,977.12 |    0
```

## Version History
+ **1.0**
	+ Initial release.

## Contributing
Please report issues or better yet, fork, fix and send a pull request.

## Author
**Soheil Rashidi**

+ http://soheilrashidi.com
+ http://twitter.com/soheilpro
+ http://github.com/soheilpro

## Copyright and License
Copyright 2018 Soheil Rashidi.

Licensed under the The MIT License (the "License");
you may not use this work except in compliance with the License.
You may obtain a copy of the License in the LICENSE file, or at:

http://www.opensource.org/licenses/mit-license.php

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
