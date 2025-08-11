import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormsModule } from '@angular/forms';
import { CurrencyService } from '../../services/currency.service';
import { Currency, CreateCurrencyDto, ExchangeRateResponse } from '../../models/currency.model';

@Component({
    selector: 'app-currency-setup',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './currency-setup.component.html',
    styleUrls: ['./currency-setup.component.css']
})
export class CurrencySetupComponent implements OnInit {
    currencyForm: FormGroup;
    currencies: Currency[] = [];
    exchangeRate: number = 0;
    selectedCurrencyCode: string = 'USD';
    isLoadingRate: boolean = false;
    exchangeRateError: string = '';
    isLoading: boolean = false;
    editingCurrency: Currency | null = null;

    constructor(
        private fb: FormBuilder,
        private currencyService: CurrencyService
    ) {
        this.currencyForm = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(100)]],
            symbol: ['', [Validators.required, Validators.maxLength(10)]],
            exchangeRate: [{ value: '', disabled: true }]
        });
    }

    ngOnInit(): void {
        this.loadCurrencies();
        this.loadExchangeRate();
    }

    loadCurrencies(): void {
        this.isLoading = true;
        this.currencyService.getAll().subscribe({
            next: (currencies) => {
                this.currencies = currencies;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading currencies:', error);
                this.isLoading = false;
            }
        });
    }

    loadExchangeRate(): void {
        this.isLoadingRate = true;
        this.exchangeRateError = '';
        
        this.currencyService.getExchangeRate(this.selectedCurrencyCode).subscribe({
            next: (response: ExchangeRateResponse) => {
                this.exchangeRate = response.exchangeRate;
                this.currencyForm.patchValue({
                    exchangeRate: `1 PKR = ${response.exchangeRate.toFixed(4)} ${response.currencyCode}`
                });
                this.isLoadingRate = false;
            },
            error: (error) => {
                console.error('Error loading exchange rate:', error);
                this.exchangeRateError = 'Failed to load exchange rate. Please try again.';
                this.currencyForm.patchValue({
                    exchangeRate: 'Error loading rate'
                });
                this.isLoadingRate = false;
            }
        });
    }

    onCurrencyCodeChange(currencyCode: string): void {
        this.selectedCurrencyCode = currencyCode;
        this.loadExchangeRate();
    }

    refreshExchangeRate(): void {
        this.loadExchangeRate();
    }

    onSubmit(): void {
        if (this.currencyForm.valid) {
            const formValue = this.currencyForm.value;
            const currencyData: CreateCurrencyDto = {
                name: formValue.name,
                symbol: formValue.symbol
            };

            if (this.editingCurrency) {
                const updateData = {
                    ...currencyData,
                    id: this.editingCurrency.id!,
                    isActive: this.editingCurrency.isActive
                };
                
                this.currencyService.update(this.editingCurrency.id!, updateData).subscribe({
                    next: () => {
                        this.loadCurrencies();
                        this.resetForm();
                    },
                    error: (error) => {
                        console.error('Error updating currency:', error);
                    }
                });
            } else {
                this.currencyService.create(currencyData).subscribe({
                    next: () => {
                        this.loadCurrencies();
                        this.resetForm();
                    },
                    error: (error) => {
                        console.error('Error creating currency:', error);
                    }
                });
            }
        }
    }

    editCurrency(currency: Currency): void {
        this.editingCurrency = currency;
        this.currencyForm.patchValue({
            name: currency.name,
            symbol: currency.symbol
        });
    }

    deleteCurrency(id: number): void {
        if (confirm('Are you sure you want to delete this currency?')) {
            this.currencyService.delete(id).subscribe({
                next: () => {
                    this.loadCurrencies();
                },
                error: (error) => {
                    console.error('Error deleting currency:', error);
                }
            });
        }
    }

    resetForm(): void {
        this.currencyForm.reset();
        this.editingCurrency = null;
        this.loadExchangeRate();
    }

    get isFormValid(): boolean {
        return this.currencyForm.valid;
    }
}