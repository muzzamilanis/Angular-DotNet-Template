import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TextInput,
  TouchableOpacity,
  Switch,
  Alert,
  Modal,
  SafeAreaView,
  Platform,
  Dimensions,
  Animated,
  ActivityIndicator,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';

const { width } = Dimensions.get('window');

interface SaleOrder {
  id: string;
  orderNumber: string;
  orderDate: string;
  businessType: 'Digital Marketing' | 'Traditional';
  clientName: string;
  contactEmail: string;
  contactPhone: string;
  projectName: string;
  serviceType: string;
  description: string;
  budget: string;
  currency: string;
  status: 'Draft' | 'Active' | 'Completed' | 'Cancelled';
}

const OrdersScreen: React.FC = () => {
  const [orders, setOrders] = useState<SaleOrder[]>([]);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [isDigitalMarketing, setIsDigitalMarketing] = useState(true);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<Partial<SaleOrder>>({
    businessType: 'Digital Marketing',
    orderDate: new Date().toISOString().split('T')[0],
    currency: 'USD',
    status: 'Draft',
  });

  const slideAnim = new Animated.Value(0);

  useEffect(() => {
    // Load existing orders (mock data for demo)
    loadOrders();
  }, []);

  const loadOrders = () => {
    // Mock data - in real app this would be API call
    const mockOrders: SaleOrder[] = [
      {
        id: '1',
        orderNumber: 'ORD-2024-001',
        orderDate: '2024-01-15',
        businessType: 'Digital Marketing',
        clientName: 'TechCorp Solutions',
        contactEmail: 'contact@techcorp.com',
        contactPhone: '+1-555-0123',
        projectName: 'Website Redesign',
        serviceType: 'Web Development',
        description: 'Complete website redesign with modern UI/UX',
        budget: '15000',
        currency: 'USD',
        status: 'Active',
      },
      {
        id: '2',
        orderNumber: 'ORD-2024-002',
        orderDate: '2024-01-20',
        businessType: 'Traditional',
        clientName: 'Local Restaurant',
        contactEmail: 'info@restaurant.com',
        contactPhone: '+1-555-0456',
        projectName: 'Marketing Campaign',
        serviceType: 'Marketing',
        description: 'Social media marketing and branding',
        budget: '5000',
        currency: 'USD',
        status: 'Draft',
      },
    ];
    setOrders(mockOrders);
  };

  const openCreateForm = () => {
    Animated.spring(slideAnim, {
      toValue: 1,
      useNativeDriver: true,
    }).start();
    setShowCreateForm(true);
  };

  const closeCreateForm = () => {
    Animated.spring(slideAnim, {
      toValue: 0,
      useNativeDriver: true,
    }).start(() => {
      setShowCreateForm(false);
      setFormData({
        businessType: 'Digital Marketing',
        orderDate: new Date().toISOString().split('T')[0],
        currency: 'USD',
        status: 'Draft',
      });
    });
  };

  const handleSaveOrder = () => {
    if (!formData.orderNumber || !formData.clientName) {
      Alert.alert('Validation Error', 'Please fill in all required fields');
      return;
    }

    setLoading(true);
    
    // Simulate API call
    setTimeout(() => {
      const newOrder: SaleOrder = {
        id: Date.now().toString(),
        orderNumber: formData.orderNumber || '',
        orderDate: formData.orderDate || '',
        businessType: formData.businessType || 'Digital Marketing',
        clientName: formData.clientName || '',
        contactEmail: formData.contactEmail || '',
        contactPhone: formData.contactPhone || '',
        projectName: formData.projectName || '',
        serviceType: formData.serviceType || '',
        description: formData.description || '',
        budget: formData.budget || '',
        currency: formData.currency || 'USD',
        status: formData.status || 'Draft',
      };

      setOrders(prev => [newOrder, ...prev]);
      setLoading(false);
      closeCreateForm();
      Alert.alert('Success', 'Sale order created successfully!');
    }, 1500);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active': return '#28a745';
      case 'Completed': return '#007bff';
      case 'Cancelled': return '#dc3545';
      default: return '#ffc107';
    }
  };

  const OrderCard = ({ order }: { order: SaleOrder }) => (
    <View style={styles.orderCard}>
      <LinearGradient
        colors={['#ffffff', '#f8f9ff']}
        style={styles.cardGradient}
      >
        <View style={styles.cardHeader}>
          <View style={styles.orderInfo}>
            <Text style={styles.orderNumber}>{order.orderNumber}</Text>
            <Text style={styles.orderDate}>{order.orderDate}</Text>
          </View>
          <View style={[styles.statusBadge, { backgroundColor: getStatusColor(order.status) }]}>
            <Text style={styles.statusText}>{order.status}</Text>
          </View>
        </View>
        
        <View style={styles.cardBody}>
          <Text style={styles.clientName}>{order.clientName}</Text>
          <Text style={styles.projectName}>{order.projectName}</Text>
          <Text style={styles.businessType}>{order.businessType}</Text>
          
          <View style={styles.cardFooter}>
            <Text style={styles.budget}>{order.currency} {order.budget}</Text>
            <TouchableOpacity style={styles.viewButton}>
              <Ionicons name="eye-outline" size={16} color="#667eea" />
              <Text style={styles.viewButtonText}>View</Text>
            </TouchableOpacity>
          </View>
        </View>
      </LinearGradient>
    </View>
  );

  return (
    <SafeAreaView style={styles.container}>
      <LinearGradient
        colors={['#667eea', '#764ba2']}
        style={styles.headerGradient}
      >
        <View style={styles.header}>
          <Text style={styles.headerTitle}>Sale Orders</Text>
          <TouchableOpacity style={styles.addButton} onPress={openCreateForm}>
            <Ionicons name="add" size={24} color="white" />
          </TouchableOpacity>
        </View>
      </LinearGradient>

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {orders.length === 0 ? (
          <View style={styles.emptyState}>
            <Ionicons name="receipt-outline" size={64} color="#ccc" />
            <Text style={styles.emptyTitle}>No Orders Yet</Text>
            <Text style={styles.emptySubtitle}>Create your first sale order to get started</Text>
          </View>
        ) : (
          orders.map((order) => (
            <OrderCard key={order.id} order={order} />
          ))
        )}
      </ScrollView>

      <Modal
        visible={showCreateForm}
        animationType="slide"
        presentationStyle="pageSheet"
      >
        <SafeAreaView style={styles.modalContainer}>
          <LinearGradient
            colors={['#667eea', '#764ba2']}
            style={styles.modalHeader}
          >
            <TouchableOpacity onPress={closeCreateForm} style={styles.closeButton}>
              <Ionicons name="close" size={24} color="white" />
            </TouchableOpacity>
            <Text style={styles.modalTitle}>Create Sale Order</Text>
            <View style={styles.placeholder} />
          </LinearGradient>

          <ScrollView style={styles.modalContent} showsVerticalScrollIndicator={false}>
            <View style={styles.formSection}>
              <Text style={styles.sectionTitle}>📋 Basic Information</Text>
              
              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Order Number *</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter order number"
                  value={formData.orderNumber}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, orderNumber: text }))}
                />
              </View>

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Order Date *</Text>
                <TextInput
                  style={styles.input}
                  placeholder="YYYY-MM-DD"
                  value={formData.orderDate}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, orderDate: text }))}
                />
              </View>

              <View style={styles.switchGroup}>
                <Text style={styles.inputLabel}>Business Type</Text>
                <View style={styles.switchContainer}>
                  <Text style={styles.switchLabel}>Traditional</Text>
                  <Switch
                    value={isDigitalMarketing}
                    onValueChange={(value) => {
                      setIsDigitalMarketing(value);
                      setFormData(prev => ({ 
                        ...prev, 
                        businessType: value ? 'Digital Marketing' : 'Traditional' 
                      }));
                    }}
                    trackColor={{ false: '#ccc', true: '#667eea' }}
                    thumbColor={isDigitalMarketing ? '#764ba2' : '#f4f3f4'}
                  />
                  <Text style={styles.switchLabel}>Digital Marketing</Text>
                </View>
              </View>
            </View>

            <View style={styles.formSection}>
              <Text style={styles.sectionTitle}>👤 Client Information</Text>
              
              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Client Name *</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter client name"
                  value={formData.clientName}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, clientName: text }))}
                />
              </View>

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Contact Email</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter contact email"
                  value={formData.contactEmail}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, contactEmail: text }))}
                  keyboardType="email-address"
                  autoCapitalize="none"
                />
              </View>

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Contact Phone</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter contact phone"
                  value={formData.contactPhone}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, contactPhone: text }))}
                  keyboardType="phone-pad"
                />
              </View>
            </View>

            <View style={styles.formSection}>
              <Text style={styles.sectionTitle}>🚀 Project Information</Text>
              
              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Project Name</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter project name"
                  value={formData.projectName}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, projectName: text }))}
                />
              </View>

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Service Type</Text>
                <TextInput
                  style={styles.input}
                  placeholder="Enter service type"
                  value={formData.serviceType}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, serviceType: text }))}
                />
              </View>

              <View style={styles.inputGroup}>
                <Text style={styles.inputLabel}>Description</Text>
                <TextInput
                  style={[styles.input, styles.textArea]}
                  placeholder="Enter project description"
                  value={formData.description}
                  onChangeText={(text) => setFormData(prev => ({ ...prev, description: text }))}
                  multiline
                  numberOfLines={4}
                />
              </View>
            </View>

            <View style={styles.formSection}>
              <Text style={styles.sectionTitle}>💰 Financial Information</Text>
              
              <View style={styles.rowInputs}>
                <View style={[styles.inputGroup, { flex: 2 }]}>
                  <Text style={styles.inputLabel}>Budget</Text>
                  <TextInput
                    style={styles.input}
                    placeholder="Enter budget amount"
                    value={formData.budget}
                    onChangeText={(text) => setFormData(prev => ({ ...prev, budget: text }))}
                    keyboardType="numeric"
                  />
                </View>
                
                <View style={[styles.inputGroup, { flex: 1 }]}>
                  <Text style={styles.inputLabel}>Currency</Text>
                  <TextInput
                    style={styles.input}
                    placeholder="USD"
                    value={formData.currency}
                    onChangeText={(text) => setFormData(prev => ({ ...prev, currency: text }))}
                  />
                </View>
              </View>
            </View>

            <TouchableOpacity 
              style={styles.saveButton} 
              onPress={handleSaveOrder}
              disabled={loading}
            >
              <LinearGradient
                colors={['#667eea', '#764ba2']}
                style={styles.saveButtonGradient}
              >
                {loading ? (
                  <ActivityIndicator color="white" />
                ) : (
                  <>
                    <Ionicons name="checkmark" size={20} color="white" />
                    <Text style={styles.saveButtonText}>Create Order</Text>
                  </>
                )}
              </LinearGradient>
            </TouchableOpacity>

            <View style={{ height: 100 }} />
          </ScrollView>
        </SafeAreaView>
      </Modal>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  headerGradient: {
    paddingTop: Platform.OS === 'ios' ? 0 : 25,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 20,
    paddingVertical: 15,
  },
  headerTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    color: 'white',
  },
  addButton: {
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    borderRadius: 20,
    padding: 8,
  },
  content: {
    flex: 1,
    paddingHorizontal: 20,
    paddingTop: 20,
  },
  orderCard: {
    marginBottom: 15,
    borderRadius: 20,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 5,
  },
  cardGradient: {
    padding: 20,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: 15,
  },
  orderInfo: {
    flex: 1,
  },
  orderNumber: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 4,
  },
  orderDate: {
    fontSize: 14,
    color: '#666',
  },
  statusBadge: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 20,
  },
  statusText: {
    color: 'white',
    fontSize: 12,
    fontWeight: 'bold',
  },
  cardBody: {
    marginBottom: 15,
  },
  clientName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
    marginBottom: 4,
  },
  projectName: {
    fontSize: 14,
    color: '#667eea',
    marginBottom: 4,
  },
  businessType: {
    fontSize: 12,
    color: '#764ba2',
    marginBottom: 10,
  },
  cardFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  budget: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#28a745',
  },
  viewButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(102, 126, 234, 0.1)',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 15,
  },
  viewButtonText: {
    color: '#667eea',
    fontSize: 12,
    fontWeight: '600',
    marginLeft: 4,
  },
  emptyState: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 80,
  },
  emptyTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
    marginTop: 20,
    marginBottom: 8,
  },
  emptySubtitle: {
    fontSize: 14,
    color: '#666',
    textAlign: 'center',
    paddingHorizontal: 40,
  },
  modalContainer: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  modalHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 20,
    paddingVertical: 15,
  },
  closeButton: {
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    borderRadius: 20,
    padding: 8,
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: 'white',
  },
  placeholder: {
    width: 40,
  },
  modalContent: {
    flex: 1,
    paddingHorizontal: 20,
  },
  formSection: {
    backgroundColor: 'white',
    borderRadius: 20,
    padding: 20,
    marginTop: 20,
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 20,
  },
  inputGroup: {
    marginBottom: 20,
  },
  inputLabel: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
    marginBottom: 8,
  },
  input: {
    borderWidth: 2,
    borderColor: '#e0e0e0',
    borderRadius: 12,
    paddingHorizontal: 16,
    paddingVertical: 12,
    fontSize: 16,
    backgroundColor: '#f8f9ff',
  },
  textArea: {
    height: 100,
    textAlignVertical: 'top',
  },
  switchGroup: {
    marginBottom: 20,
  },
  switchContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: '#f0f0f0',
    borderRadius: 25,
    paddingHorizontal: 20,
    paddingVertical: 10,
  },
  switchLabel: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
  },
  rowInputs: {
    flexDirection: 'row',
    gap: 15,
  },
  saveButton: {
    marginTop: 30,
    marginHorizontal: 20,
    borderRadius: 25,
    overflow: 'hidden',
  },
  saveButtonGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 15,
    paddingHorizontal: 30,
  },
  saveButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
    marginLeft: 8,
  },
});

export default OrdersScreen;