import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  TextInput,
  Modal,
  FlatList,
  Alert,
  Animated,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';

interface SaleOrder {
  id: string;
  orderNumber: string;
  orderDate: string;
  customerName: string;
  totalAmount: number;
  status: 'pending' | 'processing' | 'completed' | 'cancelled';
  items: OrderItem[];
}

interface OrderItem {
  id: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

const OrdersScreen: React.FC = () => {
  const [orders, setOrders] = useState<SaleOrder[]>([]);
  const [isCreateModalVisible, setIsCreateModalVisible] = useState(false);
  const [newOrder, setNewOrder] = useState<Partial<SaleOrder>>({
    orderNumber: '',
    customerName: '',
    totalAmount: 0,
    status: 'pending',
    items: [],
  });
  const [fadeAnim] = useState(new Animated.Value(0));

  useEffect(() => {
    // Animate screen entrance
    Animated.timing(fadeAnim, {
      toValue: 1,
      duration: 800,
      useNativeDriver: true,
    }).start();

    // Load sample data
    loadSampleOrders();
  }, []);

  const loadSampleOrders = () => {
    const sampleOrders: SaleOrder[] = [
      {
        id: '1',
        orderNumber: 'ORD-2024-001',
        orderDate: '2024-01-15',
        customerName: 'Acme Corporation',
        totalAmount: 2500.00,
        status: 'completed',
        items: [
          { id: '1', productName: 'Premium Service Package', quantity: 1, unitPrice: 2500.00, totalPrice: 2500.00 }
        ]
      },
      {
        id: '2',
        orderNumber: 'ORD-2024-002',
        orderDate: '2024-01-18',
        customerName: 'Tech Solutions Inc',
        totalAmount: 1800.00,
        status: 'processing',
        items: [
          { id: '2', productName: 'Digital Marketing Campaign', quantity: 1, unitPrice: 1800.00, totalPrice: 1800.00 }
        ]
      },
      {
        id: '3',
        orderNumber: 'ORD-2024-003',
        orderDate: '2024-01-20',
        customerName: 'Global Enterprises',
        totalAmount: 3200.00,
        status: 'pending',
        items: [
          { id: '3', productName: 'Consulting Services', quantity: 2, unitPrice: 1600.00, totalPrice: 3200.00 }
        ]
      }
    ];
    setOrders(sampleOrders);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed': return '#28a745';
      case 'processing': return '#ffc107';
      case 'pending': return '#6c757d';
      case 'cancelled': return '#dc3545';
      default: return '#6c757d';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'completed': return 'checkmark-circle';
      case 'processing': return 'time';
      case 'pending': return 'hourglass';
      case 'cancelled': return 'close-circle';
      default: return 'help-circle';
    }
  };

  const handleCreateOrder = () => {
    if (!newOrder.orderNumber || !newOrder.customerName) {
      Alert.alert('Error', 'Please fill in all required fields');
      return;
    }

    const order: SaleOrder = {
      id: Date.now().toString(),
      orderNumber: newOrder.orderNumber!,
      orderDate: new Date().toISOString().split('T')[0],
      customerName: newOrder.customerName!,
      totalAmount: newOrder.totalAmount || 0,
      status: 'pending',
      items: []
    };

    setOrders([order, ...orders]);
    setNewOrder({
      orderNumber: '',
      customerName: '',
      totalAmount: 0,
      status: 'pending',
      items: [],
    });
    setIsCreateModalVisible(false);
    Alert.alert('Success', 'Order created successfully!');
  };

  const renderOrderCard = ({ item }: { item: SaleOrder }) => (
    <TouchableOpacity style={styles.orderCard} activeOpacity={0.8}>
      <LinearGradient
        colors={['#667eea', '#764ba2']}
        start={{x: 0, y: 0}}
        end={{x: 1, y: 0}}
        style={styles.cardHeader}
      >
        <View style={styles.colorStrip} />
        <View style={styles.headerContent}>
          <Text style={styles.orderNumber}>{item.orderNumber}</Text>
          <View style={styles.statusContainer}>
            <View style={[styles.statusBadge, { backgroundColor: getStatusColor(item.status) }]}>
              <Ionicons 
                name={getStatusIcon(item.status) as any} 
                size={14} 
                color="white" 
                style={styles.statusIcon} 
              />
              <Text style={styles.statusText}>{item.status.toUpperCase()}</Text>
            </View>
          </View>
        </View>
      </LinearGradient>
      
      <View style={styles.cardBody}>
        <View style={styles.orderInfo}>
          <View style={styles.infoRow}>
            <Ionicons name="person" size={16} color="#667eea" />
            <Text style={styles.customerName}>{item.customerName}</Text>
          </View>
          <View style={styles.infoRow}>
            <Ionicons name="calendar" size={16} color="#667eea" />
            <Text style={styles.orderDate}>{item.orderDate}</Text>
          </View>
          <View style={styles.infoRow}>
            <Ionicons name="cash" size={16} color="#667eea" />
            <Text style={styles.totalAmount}>${item.totalAmount.toFixed(2)}</Text>
          </View>
        </View>
        
        <TouchableOpacity style={styles.viewButton}>
          <Text style={styles.viewButtonText}>View Details</Text>
          <Ionicons name="chevron-forward" size={16} color="#667eea" />
        </TouchableOpacity>
      </View>
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <Animated.View style={[styles.content, { opacity: fadeAnim }]}>
        {/* Header */}
        <LinearGradient
          colors={['#667eea', '#764ba2']}
          start={{x: 0, y: 0}}
          end={{x: 1, y: 0}}
          style={styles.header}
        >
          <View style={styles.headerColorStrip} />
          <View style={styles.headerTitleContainer}>
            <Ionicons name="receipt-outline" size={28} color="white" />
            <Text style={styles.headerTitle}>Sale Orders</Text>
          </View>
          <TouchableOpacity 
            style={styles.addButton}
            onPress={() => setIsCreateModalVisible(true)}
          >
            <Ionicons name="add" size={24} color="white" />
          </TouchableOpacity>
        </LinearGradient>

        {/* Orders List */}
        <View style={styles.ordersContainer}>
          {orders.length === 0 ? (
            <View style={styles.emptyState}>
              <Ionicons name="receipt-outline" size={64} color="#ccc" />
              <Text style={styles.emptyStateText}>No orders found</Text>
              <Text style={styles.emptyStateSubtext}>Create your first order to get started</Text>
            </View>
          ) : (
            <FlatList
              data={orders}
              renderItem={renderOrderCard}
              keyExtractor={(item) => item.id}
              showsVerticalScrollIndicator={false}
              contentContainerStyle={styles.listContainer}
            />
          )}
        </View>
      </Animated.View>

      {/* Create Order Modal */}
      <Modal
        visible={isCreateModalVisible}
        animationType="slide"
        presentationStyle="formSheet"
      >
        <View style={styles.modalContainer}>
          <LinearGradient
            colors={['#667eea', '#764ba2']}
            start={{x: 0, y: 0}}
            end={{x: 1, y: 0}}
            style={styles.modalHeader}
          >
            <View style={styles.modalHeaderColorStrip} />
            <Text style={styles.modalTitle}>Create New Order</Text>
            <TouchableOpacity
              style={styles.closeButton}
              onPress={() => setIsCreateModalVisible(false)}
            >
              <Ionicons name="close" size={24} color="white" />
            </TouchableOpacity>
          </LinearGradient>

          <ScrollView style={styles.modalContent}>
            <View style={styles.formGroup}>
              <Text style={styles.label}>
                <Ionicons name="hashtag" size={16} color="#667eea" /> Order Number *
              </Text>
              <TextInput
                style={styles.input}
                value={newOrder.orderNumber}
                onChangeText={(text) => setNewOrder({...newOrder, orderNumber: text})}
                placeholder="Enter order number"
                placeholderTextColor="#999"
              />
            </View>

            <View style={styles.formGroup}>
              <Text style={styles.label}>
                <Ionicons name="person" size={16} color="#667eea" /> Customer Name *
              </Text>
              <TextInput
                style={styles.input}
                value={newOrder.customerName}
                onChangeText={(text) => setNewOrder({...newOrder, customerName: text})}
                placeholder="Enter customer name"
                placeholderTextColor="#999"
              />
            </View>

            <View style={styles.formGroup}>
              <Text style={styles.label}>
                <Ionicons name="cash" size={16} color="#667eea" /> Total Amount
              </Text>
              <TextInput
                style={styles.input}
                value={newOrder.totalAmount?.toString() || ''}
                onChangeText={(text) => setNewOrder({...newOrder, totalAmount: parseFloat(text) || 0})}
                placeholder="Enter total amount"
                placeholderTextColor="#999"
                keyboardType="numeric"
              />
            </View>

            <TouchableOpacity style={styles.createButton} onPress={handleCreateOrder}>
              <LinearGradient
                colors={['#667eea', '#764ba2']}
                start={{x: 0, y: 0}}
                end={{x: 1, y: 0}}
                style={styles.createButtonGradient}
              >
                <Ionicons name="add-circle" size={20} color="white" />
                <Text style={styles.createButtonText}>Create Order</Text>
              </LinearGradient>
            </TouchableOpacity>
          </ScrollView>
        </View>
      </Modal>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  content: {
    flex: 1,
  },
  header: {
    paddingTop: 60,
    paddingHorizontal: 20,
    paddingBottom: 20,
    position: 'relative',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  headerColorStrip: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    height: 4,
    backgroundColor: '#ff6b6b',
    opacity: 0.8,
  },
  headerTitleContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  headerTitle: {
    fontSize: 24,
    fontWeight: '600',
    color: 'white',
    marginLeft: 12,
    textShadowColor: 'rgba(0, 0, 0, 0.1)',
    textShadowOffset: { width: 0, height: 1 },
    textShadowRadius: 2,
  },
  addButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    alignItems: 'center',
    justifyContent: 'center',
  },
  ordersContainer: {
    flex: 1,
    paddingHorizontal: 20,
    paddingTop: 20,
  },
  orderCard: {
    backgroundColor: 'white',
    borderRadius: 15,
    marginBottom: 16,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.08,
    shadowRadius: 20,
    elevation: 5,
  },
  cardHeader: {
    paddingHorizontal: 20,
    paddingVertical: 16,
    position: 'relative',
  },
  colorStrip: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    height: 4,
    backgroundColor: '#feca57',
    opacity: 0.8,
  },
  headerContent: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  orderNumber: {
    fontSize: 18,
    fontWeight: '600',
    color: 'white',
    textShadowColor: 'rgba(0, 0, 0, 0.1)',
    textShadowOffset: { width: 0, height: 1 },
    textShadowRadius: 2,
  },
  statusContainer: {
    alignItems: 'flex-end',
  },
  statusBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 12,
    minWidth: 80,
    justifyContent: 'center',
  },
  statusIcon: {
    marginRight: 4,
  },
  statusText: {
    color: 'white',
    fontSize: 12,
    fontWeight: '600',
  },
  cardBody: {
    padding: 20,
  },
  orderInfo: {
    marginBottom: 16,
  },
  infoRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },
  customerName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#2c3e50',
    marginLeft: 8,
    flex: 1,
  },
  orderDate: {
    fontSize: 14,
    color: '#666',
    marginLeft: 8,
    flex: 1,
  },
  totalAmount: {
    fontSize: 16,
    fontWeight: '700',
    color: '#28a745',
    marginLeft: 8,
    flex: 1,
  },
  viewButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 12,
    paddingHorizontal: 20,
    backgroundColor: '#f8f9fa',
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#e9ecef',
  },
  viewButtonText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#667eea',
    marginRight: 4,
  },
  emptyState: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 60,
  },
  emptyStateText: {
    fontSize: 18,
    fontWeight: '600',
    color: '#666',
    marginTop: 16,
  },
  emptyStateSubtext: {
    fontSize: 14,
    color: '#999',
    marginTop: 8,
    textAlign: 'center',
  },
  listContainer: {
    paddingBottom: 20,
  },
  modalContainer: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  modalHeader: {
    paddingTop: 60,
    paddingHorizontal: 20,
    paddingBottom: 20,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    position: 'relative',
  },
  modalHeaderColorStrip: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    height: 4,
    backgroundColor: '#48dbfb',
    opacity: 0.8,
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: '600',
    color: 'white',
    flex: 1,
    textAlign: 'center',
    textShadowColor: 'rgba(0, 0, 0, 0.1)',
    textShadowOffset: { width: 0, height: 1 },
    textShadowRadius: 2,
  },
  closeButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    alignItems: 'center',
    justifyContent: 'center',
  },
  modalContent: {
    flex: 1,
    paddingHorizontal: 20,
    paddingTop: 20,
  },
  formGroup: {
    marginBottom: 20,
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    color: '#2c3e50',
    marginBottom: 8,
    flexDirection: 'row',
    alignItems: 'center',
  },
  input: {
    borderWidth: 2,
    borderColor: '#e9ecef',
    borderRadius: 10,
    paddingHorizontal: 16,
    paddingVertical: 14,
    fontSize: 16,
    backgroundColor: 'white',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 3,
    elevation: 2,
  },
  createButton: {
    marginTop: 20,
    marginBottom: 40,
    borderRadius: 12,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.15,
    shadowRadius: 8,
    elevation: 8,
  },
  createButtonGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 16,
    paddingHorizontal: 24,
  },
  createButtonText: {
    fontSize: 16,
    fontWeight: '600',
    color: 'white',
    marginLeft: 8,
  },
});

export default OrdersScreen;