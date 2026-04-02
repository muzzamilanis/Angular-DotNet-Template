import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
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

    /** When true, the component runs inside the slide-in popup (always creation mode) */
    @Input() isPopup = false;
    @Output() closed = new EventEmitter<void>();

    /** Set when opened with ?orderId= — indicates update mode */
    editOrderId: number | null = null;
    editOrderNumber: string | null = null;
    /** True once the target order is confirmed to be in "New" status */
    isEditLocked = false;

    constructor(
        private orderWorkflowService: OrderWorkflowService,
        private saleOrderService: SaleOrderService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        // In popup mode, always start in creation mode — skip route params entirely
        if (this.isPopup) {
            this.messages.push({
                role: 'assistant',
                text: '👋 Hi! Describe the new order in plain language and I\'ll structure it for you.\n\nExample: "Write a 2000-word essay on climate change for John, deadline in 7 days."'
            });
            return;
        }

        this.route.queryParamMap.subscribe(params => {
            const idParam = params.get('orderId');
            if (idParam && !Number.isNaN(+idParam)) {
                this.editOrderId = +idParam;
                this.isLoading = true;
                this.saleOrderService.getSaleOrder(this.editOrderId).subscribe({
                    next: (order) => {
                        this.isLoading = false;
                        this.editOrderNumber = order.orderNumber;
                        if (order.status !== 'New') {
                            this.isEditLocked = true;
                            this.messages.push({
                                role: 'assistant',
                                text: `🔒 Order #${order.orderNumber} is locked (status: ${order.status}). Orders can only be modified while in "New" status.`
                            });
                        } else {
                            this.messages.push({
                                role: 'assistant',
                                text: `✏️ You are editing Order #${order.orderNumber}.\n\nDescribe what you want to change and I'll update the order.\n\nExample: "Change client to Ahmed, update deadline to April 15, total is 500 USD"`
                            });
                        }
                    },
                    error: () => {
                        this.isLoading = false;
                        this.isEditLocked = true;
                        this.messages.push({
                            role: 'assistant',
                            text: '❌ Could not load the order. Please go back and try again.'
                        });
                    }
                });
            } else {
                // Create mode — default greeting
                this.messages.push({
                    role: 'assistant',
                    text: '👋 Hi! Describe the new order in plain language and I\'ll structure it for you.\n\nExample: "Write a 2000-word essay on climate change for John, deadline in 7 days."'
                });
            }
        });
    }

    ngOnDestroy(): void { }

    get isUpdateMode(): boolean {
        return this.editOrderId !== null;
    }

    sendMessage(): void {
        const text = this.inputText.trim();
        if (!text || this.isLoading || this.isEditLocked) return;

        this.messages.push({ role: 'user', text });
        this.inputText = '';
        this.isLoading = true;

        if (this.isUpdateMode) {
            // In update mode, parse to preview the changes before confirming
            this.orderWorkflowService.parseOrder(text).subscribe({
                next: (parsed) => {
                    // Store the raw text alongside the parsed preview for the confirm step
                    this.pendingParsedOrder = { ...parsed, _rawText: text };
                    this.messages.push({
                        role: 'assistant',
                        text: '✅ Here\'s what I understood from your update request:',
                        parsedOrder: parsed
                    });
                    this.isLoading = false;
                },
                error: (err) => {
                    this.messages.push({
                        role: 'assistant',
                        text: '❌ Sorry, I could not parse that. Please try again.'
                    });
                    this.isLoading = false;
                    console.error('Parse order error:', err);
                }
            });
        } else {
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
            csrChatText: this.pendingParsedOrder.originalText ?? null,
            items: []
        };

        this.saleOrderService.createSaleOrder(orderPayload as any).subscribe({
            next: (created) => {
                this.isLoading = false;
                this.messages.push({
                    role: 'assistant',
                    text: `🎉 Order #${created.id} created! Redirecting to live workflow tracker...`
                });
                setTimeout(() => {
                    if (this.isPopup) this.closed.emit();
                    this.router.navigate(['/order-workflow', created.id]);
                }, 1500);
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

    confirmUpdate(): void {
        if (!this.pendingParsedOrder || !this.editOrderId) return;
        this.isLoading = true;
        const rawText = this.pendingParsedOrder._rawText as string;

        this.orderWorkflowService.updateOrderViaChat(this.editOrderId, rawText).subscribe({
            next: (updated) => {
                this.isLoading = false;
                this.pendingParsedOrder = null;
                this.messages.push({
                    role: 'assistant',
                    text: `✅ Order #${updated.orderNumber} has been updated successfully! You can describe more changes or go back.`
                });
            },
            error: (err) => {
                this.isLoading = false;
                if (err?.status === 409) {
                    this.isEditLocked = true;
                    this.messages.push({
                        role: 'assistant',
                        text: `🔒 This order is now locked and cannot be modified. Its status has changed since you opened this page.`
                    });
                } else {
                    const msg: string = typeof err?.error === 'string'
                        ? (err.error || 'Failed to update the order.')
                        : (err?.error?.message
                            ?? err?.error?.title
                            ?? err?.error?.detail
                            ?? err?.message
                            ?? 'Failed to update the order.');
                    this.messages.push({ role: 'assistant', text: `❌ ${msg}` });
                }
                console.error('Update order error:', err);
            }
        });
    }

    close(): void {
        this.closed.emit();
    }

    cancelOrder(): void {
        this.messages.push({
            role: 'assistant',
            text: this.isUpdateMode
                ? '↩️ Update cancelled. Describe another change or go back.'
                : '↩️ Order cancelled. Please describe the order again or make changes.'
        });
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
