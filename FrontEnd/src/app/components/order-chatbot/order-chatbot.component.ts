import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { SaleOrderService } from '../../services/sale-order.service';
import { OrderWorkflowService } from '../../services/order-workflow.service';

interface ChatMessage {
    role: 'user' | 'assistant';
    text: string;
    parsedOrder?: any;
}

@Component({
    selector: 'app-order-chatbot',
    templateUrl: './order-chatbot.component.html',
    styleUrls: ['./order-chatbot.component.css'],
    standalone: false,
})
export class OrderChatbotComponent implements OnInit, OnDestroy {
    messages: ChatMessage[] = [];
    inputText = '';
    isLoading = false;
    pendingParsedOrder: any = null;

    constructor(
        private orderWorkflowService: OrderWorkflowService,
        private saleOrderService: SaleOrderService,
        private router: Router
    ) { }

    ngOnInit(): void {
        this.messages.push({
            role: 'assistant',
            text: '👋 Hi! Describe the new order in plain language and I\'ll structure it for you.\n\nExample: "Write a 2000-word essay on climate change for John, deadline in 7 days."'
        });
    }

    ngOnDestroy(): void { }

    sendMessage(): void {
        const text = this.inputText.trim();
        if (!text || this.isLoading) return;

        this.messages.push({ role: 'user', text });
        this.inputText = '';
        this.isLoading = true;

        this.orderWorkflowService.parseOrder(text).subscribe({
            next: (parsed) => {
                this.pendingParsedOrder = parsed;
                this.messages.push({
                    role: 'assistant',
                    text: '✅ Here\'s what I understood from your description:',
                    parsedOrder: parsed
                });
                this.isLoading = false;
            },
            error: (err) => {
                this.messages.push({
                    role: 'assistant',
                    text: '❌ Sorry, I could not parse that order. Please try again or enter the order manually.'
                });
                this.isLoading = false;
                console.error('Parse order error:', err);
            }
        });
    }

    confirmOrder(): void {
        if (!this.pendingParsedOrder) return;
        console.log('Creating order with payload:', this.pendingParsedOrder);
        this.isLoading = true;
        const orderPayload = {
            orderNumber: this.pendingParsedOrder.suggestedOrderId ?? `SO-${Date.now()}`,
            orderDate: new Date().toISOString(),
            orderDetails: this.pendingParsedOrder.suggestedOrderDetails,
            clientName: this.pendingParsedOrder.suggestedClientName,
            customerName: this.pendingParsedOrder.suggestedClientName,
            customerAddress: '',
            contactEmail: this.pendingParsedOrder.suggestedContactEmail,
            contactPhone: this.pendingParsedOrder.suggestedContactPhone,
            deadline: this.pendingParsedOrder.suggestedDeadline,
            wordsOrPages: this.pendingParsedOrder.suggestedWordCount,
            country: this.pendingParsedOrder.suggestedCountry,
            currencyCode: this.pendingParsedOrder.suggestedCurrencyCode,
            foreignCurrencyAmount: this.pendingParsedOrder.suggestedCurrencyAmount,
            pkrAmount: this.pendingParsedOrder.suggestedPkrCurrencyAmount,
            exchangeRateUsed: this.pendingParsedOrder.suggestedExchangeRate,
            amountRecieved: this.pendingParsedOrder.suggestedAmountRecieved,
            amountReceivedInPkr: this.pendingParsedOrder.suggestedAmountReceivedInPkr,
            projectId: this.pendingParsedOrder.suggestedProjectId,
            serviceTypeId: this.pendingParsedOrder.suggestedServiceTypeId,
            status: 'New',
            paymentStatus: this.pendingParsedOrder.paymentStatus ?? 'Pending',
            totalPaymentAmount: this.pendingParsedOrder.totalPaymentAmount ?? 0,
            sourceOfPayment: this.pendingParsedOrder.sourceOfPayment ?? null,
            items: []
        };

        this.saleOrderService.createSaleOrder(orderPayload as any).subscribe({
            next: (created) => {
                this.isLoading = false;
                this.messages.push({
                    role: 'assistant',
                    text: `🎉 Order #${created.id} created! Redirecting to live workflow tracker...`
                });
                setTimeout(() => this.router.navigate(['/order-workflow', created.id]), 1500);
            },
            error: (err) => {
                this.isLoading = false;
                this.messages.push({
                    role: 'assistant',
                    text: '❌ Failed to create the order. Please try again.'
                });
                console.error('Create order error:', err);
            }
        });
    }

    cancelOrder(): void {
        this.pendingParsedOrder = null;
        this.messages.push({ role: 'assistant', text: '↩️ Order cancelled. Please describe the order again or make changes.' });
    }

    onKeyDown(event: KeyboardEvent): void {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            this.sendMessage();
        }
    }

    trackByIndex(index: number): number {
        return index;
    }
}
